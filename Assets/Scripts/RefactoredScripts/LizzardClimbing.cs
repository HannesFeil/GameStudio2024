using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class LizzardClimbing : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private LayerMask whatIsWall;
    [SerializeField]
    private AnimalType aniaml = AnimalType.LIZARD;

    private Rigidbody _rb;
    private PlayerMovement _pm;

    [Header("Climbing")]
    [SerializeField]
    private float climbSpeed = 10;
    [SerializeField]
    private float staminaDrain = 2f;

    [Header("ClimbJumping")]
    [SerializeField]
    private float climbJumpUpForce = 3;
    [SerializeField]
    private float climbJumpBackForce = 7;
    [SerializeField]
    private float climbJumpCD = 0.5f;
    private float _climbJumpCDTimer;


    [Header("Detection")]
    [SerializeField]
    private float detectionLength = 0.7f;
    [SerializeField]
    private float sphereCastRadiu = 0.25f;
    [SerializeField]
    private float maxWallLookAngle = 120;

    private float _wallLookAngle;

    private RaycastHit _frontWallHit;
    private bool _wallFront;

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

    private Transform _lastWall;
    private Vector3 _lastWallNormal;
    [SerializeField]
    private float minWallAngleChange;

    private Vector3 _moveDirection;

    public void Setup()
    {
        _pm = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody>();
    }

    public void SwitchOf()
    {
        StopClimbing();
        this.enabled = false;
    }

    private void Update()
    {
        MyInput();
        WallDetection();
        StateMachine();

        _climbJumpCDTimer = Mathf.Max(_climbJumpCDTimer - Time.deltaTime, 0);
    }

    private void FixedUpdate()
    {
        if (_pm.IsClimbing())
        {
            ClimbingMovment();
        }
    }

    private void StateMachine()
    {
        if(_wallFront && _specialInput && _wallLookAngle < maxWallLookAngle && _pm.GetStamina((int) aniaml) > 0)
        {
            if (!_pm.IsClimbing())
            {
                StartClimbing();
            }
            _pm.SetStamina((int) aniaml, _pm.GetStamina((int) aniaml) - (staminaDrain * Time.deltaTime));
        } 
        else
        {
            if (_pm.IsClimbing())
            {
                StopClimbing();
            }
        }

        if(_wallFront && _jumpInput && _pm.IsClimbing())
        {
            ClimbJump();
        }
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _jumpInput = Input.GetKey(jumpKey);
        _specialInput = Input.GetKey(SpecialKey);
    }

    private void WallDetection()
    {
        if (_pm.IsClimbing())
        {
            _wallFront = Physics.SphereCast(transform.position, sphereCastRadiu, -_frontWallHit.normal, out _frontWallHit, detectionLength, whatIsWall);
        } else
        {
            _wallFront = Physics.SphereCast(transform.position, sphereCastRadiu, orientation.forward, out _frontWallHit, detectionLength, whatIsWall);
        }

        bool newWall = _frontWallHit.transform != _lastWall || Mathf.Abs(Vector3.Angle(_lastWallNormal,_frontWallHit.normal)) > minWallAngleChange;
        _wallLookAngle = Vector3.Angle(orientation.forward, -_frontWallHit.normal);
    }

    private void StartClimbing()
    {
        _rb.useGravity = false;
        _pm.SetClimbing(true);

        _lastWall = _frontWallHit.transform;
        _lastWallNormal = _frontWallHit.normal;
    }

    private void ClimbingMovment()
    {
        Vector3 dirHorizontal = Quaternion.Euler(new Vector3(0f,-90f,0f)) * new Vector3(_frontWallHit.normal.x,0f, _frontWallHit.normal.z);
        _moveDirection = Vector3.up * _verticalInput + dirHorizontal * _horizontalInput;

        _rb.AddForce(AngleMoveDirection() * climbSpeed * 10, ForceMode.Force);
    }

    private Vector3 AngleMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _frontWallHit.normal).normalized;
    }

    private void ClimbJump()
    {
        if (_climbJumpCDTimer > 0) return;

        _climbJumpCDTimer = climbJumpCD;
        StopClimbing();
        Vector3 forceToApply = transform.up * climbJumpUpForce + _frontWallHit.normal * climbJumpBackForce;

        _rb.velocity = new Vector3(_rb.velocity.x,0f,_rb.velocity.z);
        _rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    private void StopClimbing()
    {
        _rb.useGravity = true;
        _pm.SetClimbing(false);
    }
}
