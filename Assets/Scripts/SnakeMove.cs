using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMove : AnimalMove
{
    [SerializeField] private Grappling gp;

    public bool grappling;

    private bool enableMovementOnNextTouch;

    public override void Move(Vector2 dir, bool specialActive)
    {
        if (grappling) return;

        if (!pm.IsGrounded())
        {
            dir *= pm.airMovementFactor;
        }
        pm.rb.AddForce(dir.x * pm.movementForce, 0, dir.y * pm.movementForce);

        if (specialActive)
        {
            gp.StartGrapple();

        }
    }

    public void JumpToPosition(Vector3 target, float trajectoryHeight)
    {
        grappling = true;
        velocityToSet = CalculateJumpVelocity(transform.position, target, trajectoryHeight);
        Invoke(nameof(setVelocity), 0.1f);

        Invoke(nameof(turnOf), 2f);
    }

    private void turnOf()
    {
        grappling = false;
        pm.rb.drag = 1f;
    }

    private Vector3 velocityToSet;

    private void setVelocity()
    {
        enableMovementOnNextTouch = true;
        pm.rb.drag = 0f;
        pm.rb.velocity = velocityToSet;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enableMovementOnNextTouch)
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
}
