using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Swing : MonoBehaviour
{
    private SnakeMove _sm;
    private PlayerManagement _pm;

    public Transform tip;
    public LayerMask whatToGrapple;
    public LineRenderer lr;

    [SerializeField] float maxDistance;
    private Vector3 _currentGrapplePosition;
    private Vector3 _swingPoint;
    private SpringJoint _joint;

    public bool Swingin;

    [SerializeField]
    float hThrustForce;

    [SerializeField]
    float fThrustForce;

    public RaycastHit predictionHit;
    public float predictionSphereCasRadius;
    public Transform predictionPoint;

    // Start is called before the first frame update
    public void StartSwing()
    {
        if (Swingin) return;

        if (predictionHit.point == Vector3.zero) return;

            predictionPoint.gameObject.SetActive(false);
            lr.enabled = true;
            _currentGrapplePosition = tip.position;
            Swingin = true;
            _swingPoint = predictionHit.point;
            _joint = _pm.gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = _swingPoint;
            lr.positionCount = 2;

            print(_swingPoint);

            float distanceFromPoint = Vector3.Distance(_pm.GetTransform().position, _swingPoint);

            _joint.maxDistance = distanceFromPoint * 0.8f;
            _joint.minDistance = distanceFromPoint * 0.25f;

            _joint.spring = 4.5f;
            _joint.damper = 7f;
            _joint.massScale = 4.5f;

        
    }

    public void CheckForSwingPoints()
    {
        if (_joint != null) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(_pm.GetCamTransform().position, predictionSphereCasRadius, _pm.GetCamDir(),
            out sphereCastHit, maxDistance, whatToGrapple);

        RaycastHit raycastHit;
        Physics.Raycast(_pm.GetCamTransform().position, _pm.GetCamDir(), out raycastHit, maxDistance, whatToGrapple);

        Vector3 realHitPoint;
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;
        else if(sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;
        else
            realHitPoint = Vector3.zero;

        if(realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        } else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit: raycastHit;
    }

    public void StopSwing()
    {
        _currentGrapplePosition = tip.position;
        Swingin = false;
        lr.positionCount = 0;
        Destroy(_joint);
    }

    public void OdmGearMovement()
    {
        float h = Input.GetAxis("Horizontal");
        Vector3 forces = new Vector3(h, 0, 0).normalized;
        forces = Quaternion.Euler(0, _pm.GetCamTransform().rotation.y, 0) * forces;

        _pm.GetRigidbody().AddForce(forces * hThrustForce);

        if (Input.GetButton("Jump"))
        {
            Vector3 directionToPoint = _swingPoint - _pm.GetTransform().position;
            _pm.GetRigidbody().AddForce(directionToPoint.normalized * fThrustForce);

            float distanceFromPoint = Vector3.Distance(_pm.GetTransform().position, _swingPoint);
            _joint.maxDistance = distanceFromPoint * 0.8f;
            _joint.minDistance = distanceFromPoint * 0.25f;
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        if (!_joint) return;

        _currentGrapplePosition = Vector3.Lerp(_currentGrapplePosition, _swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, tip.position);
        lr.SetPosition(1, _currentGrapplePosition);
    }
    public void SetPm(PlayerManagement pm)
    {
        _pm = pm;
    }
}
