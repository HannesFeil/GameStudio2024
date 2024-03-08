using System;
using UnityEngine;

public class SnakeMove : AnimalMove
{
    [SerializeField] private Grappling _gp;
    [SerializeField] private Swing _sw;
    [SerializeField] private float staminaDrain = 0.1f;

    private bool _enableMovementOnNextTouch;

    public override void Setup()
    {
        print("SNAKE");
        _gp.SetPm(pm);
        _sw.SetPm(pm);
    }

    public override void Move(Vector2 dir, bool specialActive)
    {
        
        pm.CamLookAtPlayer();
        UpdateRotation();
        _sw.CheckForSwingPoints();
        
        if (_gp.GrapplingNow) return;

        if (_sw.Swingin)
        {
            _sw.OdmGearMovement();
        }
        else
        {
            CheckJump();
            pm.CheckSwap();
        }



        if (!pm.IsGrounded())
        {
            dir *= pm.AirMovementFactor;
        }
        
        pm.GetRigidbody().AddForce(dir.x * pm.MovementForce, 0, dir.y * pm.MovementForce);

        if (specialActive && stamina > 0)
        {
            //gp.StartGrapple();
            _sw.StartSwing();
            stamina -= staminaDrain;
        } else 
        { 
            _sw.StopSwing();
        }
    }

    public void JumpToPosition(Vector3 target, float trajectoryHeight)
    {
        velocityToSet = CalculateJumpVelocity(transform.position, target, trajectoryHeight);
        Invoke(nameof(setVelocity), 0.1f);

        Invoke(nameof(turnOf), 2f);
    }

    private void turnOf()
    {
        pm.GetRigidbody().drag = 1f;
    }

    private Vector3 velocityToSet;

    private void setVelocity()
    {
        _enableMovementOnNextTouch = true;
        pm.GetRigidbody().drag = 0f;
        pm.GetRigidbody().velocity = velocityToSet;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!_sw.Swingin) pm.ResetDrag();
        if (_enableMovementOnNextTouch)
        {
            turnOf();
        }
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

    public override void OnSwappedFrom()
    {
        pm.ResetDrag();
    }
}
