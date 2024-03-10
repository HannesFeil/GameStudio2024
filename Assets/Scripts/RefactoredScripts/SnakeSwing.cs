using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class SnakeSwing : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform playerCam;
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private LineRenderer lr;
    [SerializeField]
    private Transform tongueTip;
    [SerializeField]
    private Transform player;
    [SerializeField]
    LayerMask whatIsGrappleable;
    [SerializeField]
    private AnimalType aniaml = AnimalType.SNAKE;

    private Rigidbody _rb;
    private PlayerMovement _pm;

    [Header("Swinging")]
    [SerializeField]
    private float maxSwingDistance = 25f;
    private Vector3 _swingPoint;
    private Vector3 _currentGrapplePosition;
    [SerializeField]
    private float horizontalThrustForce = 10f;
    [SerializeField]
    private float forwardThrustForce = 15f;
    [SerializeField]
    private float extansionSpeed = 3f;
    [SerializeField]
    private float grappleDuration = 0.2f;
    [SerializeField]
    private float grappleBoostHeight = 2f;
    [SerializeField]
    private float staminaDrain = 10f;

    [Header("Joint")]
    private SpringJoint _joint;
    [SerializeField]
    private float jointSpring = 10;
    [SerializeField]
    private float jointDamper = 7;
    [SerializeField]
    private float jointMassScale = 4.5f;

    [Header("Keybinds")]
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;
    [SerializeField]
    private KeyCode SpecialKey = KeyCode.Mouse0;

    [Header("Input")]
    private float _horizontalInput;
    private float _verticalInput;
    private bool _jumpInput;
    private bool _specialInput;

    [Header("Prediction")]
    [SerializeField]
    private float predictionSphereCasRadius = 0.4f;
    [SerializeField]
    private Transform predictionPoint;
    private RaycastHit _predictionHit;

    // Start is called before the first frame update
    public void Setup()
    {
        _rb = GetComponent<Rigidbody>();
        _pm = GetComponent<PlayerMovement>();
    }

    public void SwitchOf()
    {
        StopSwing();
        EndGrapple();
        predictionPoint.gameObject.SetActive(false);
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        CheckForSwingPoints();
    }

    private void FixedUpdate()
    {
        if (_specialInput && _pm.GetStamina((int) aniaml) > 0)
        {
            if(_pm.GetStamina((int)aniaml) > staminaDrain)
            {
                StartSwing();
            }   
        } else
        {
            StopSwing();
        }

        if (_pm.IsSwinging())
        {
            OdmGearMovement();
        }
    }
    private void LateUpdate()
    {
        DrawRope();
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _jumpInput = Input.GetKey(jumpKey);
        _specialInput = Input.GetKey(SpecialKey);
    }

    private void CheckForSwingPoints()
    {
        if (_joint != null) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(playerCam.position, predictionSphereCasRadius, playerCam.forward,
            out sphereCastHit, maxSwingDistance, whatIsGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(playerCam.position, playerCam.forward, out raycastHit, maxSwingDistance, whatIsGrappleable);

        Vector3 realHitPoint;
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;
        else
            realHitPoint = Vector3.zero;

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        _predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    private void StartSwing()
    {
        if(_pm.IsSwinging()) return;
        if (_pm.IsGrappling()) return;
        if (_predictionHit.point == Vector3.zero) return;

        _pm.SetStamina((int) aniaml, _pm.GetStamina((int) aniaml) - staminaDrain);

        predictionPoint.gameObject.SetActive(false);
        _pm.SetSwinging(true);
        _swingPoint = _predictionHit.point;
        _joint = player.gameObject.AddComponent<SpringJoint>();
        _joint.autoConfigureConnectedAnchor = false;
        _joint.connectedAnchor = _swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, _swingPoint);

        _joint.maxDistance = distanceFromPoint * 0.8f;
        _joint.minDistance = distanceFromPoint * 0.25f;

        _joint.spring = jointSpring;
        _joint.damper = jointDamper;
        _joint.massScale = jointMassScale;

        lr.positionCount = 2;
        _currentGrapplePosition = tongueTip.position;
    }

    private void OdmGearMovement()
    {
        _pm.SetStamina((int)aniaml, _pm.GetStamina((int)aniaml) - (staminaDrain/20 * Time.deltaTime));
        _rb.AddForce(orientation.right * _horizontalInput * horizontalThrustForce, ForceMode.Force);

        if(_verticalInput > 0)
        {
            _rb.AddForce(orientation.forward * _verticalInput * forwardThrustForce, ForceMode.Force);
        } else if(_verticalInput < 0)
        {
            float extendetDistanceFromPoint = Vector3.Distance(transform.position, _swingPoint) + extansionSpeed;

            _joint.maxDistance = Mathf.Min(extendetDistanceFromPoint * 0.8f, maxSwingDistance);
            _joint.minDistance = Mathf.Min(extendetDistanceFromPoint * 0.25f, maxSwingDistance);
        }

        if (_jumpInput)
        {
            StartGrapple();
        }
    }

    private void StartGrapple()
    {
        _pm.SetStamina((int)aniaml, _pm.GetStamina((int)aniaml) - staminaDrain);
        _pm.SetGrappling(true);
        StopSwing();
        Invoke(nameof(ExecuteGrapple), 0.1f);
    }

    private void ExecuteGrapple()
    {

        float grapplePointRelativeY = _swingPoint.y - transform.position.y;
        float highestPointOnArc = Mathf.Max(grapplePointRelativeY, 0) + grappleBoostHeight;
        Vector3 jumpVel = CalculateJumpVelocity(transform.position,_swingPoint, highestPointOnArc);
        _rb.drag = 0f;
        _rb.velocity = jumpVel;
        Invoke(nameof(EndGrapple),grappleDuration);
    }

    private void EndGrapple()
    {
        _pm.SetGrappling(false);
    }

    private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private void StopSwing()
    {
        lr.positionCount = 0;
        Destroy(_joint);
        _pm.SetSwinging(false);
    }

    private void DrawRope()
    {
        if (!_pm.IsSwinging()) return;

        _currentGrapplePosition = Vector3.Lerp(_currentGrapplePosition, _swingPoint, Time.deltaTime * 5f);

        lr.SetPosition(0, tongueTip.position);
        lr.SetPosition(1, _swingPoint);
    }
}
