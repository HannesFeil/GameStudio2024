using UnityEngine;

public abstract class AnimalMove : MonoBehaviour
{
    protected PlayerManagement pm;

    public virtual void Move(Vector2 dir, bool specialActive) 
    {
        pm.camLookAt(Vector3.up * pm.CamSphereRaduis, pm.CamDistance);
        checkJump();
        updateRotation();
        pm.checkSwap();
    
        if (!pm.IsGrounded()) {
            dir *= pm.AirMovementFactor;
        }
        
        pm.Rigidbody.AddForce(dir.x * pm.MovementForce, 0, dir.y * pm.MovementForce);
    }

    public void checkJump() 
    {
        if (Input.GetButton("Jump") && pm.IsGrounded()) {
            Jump();
            pm.SetNotGrounded();
        }
    }

    public void Jump() 
    {
        pm.Rigidbody.AddForce(0, pm.JumpForce, 0);
    }

    public void updateRotation() 
    {
        if (pm.Rigidbody.velocity.magnitude > 0.001) {
            Quaternion targetRotation = Quaternion.LookRotation(pm.Rigidbody.velocity, Vector3.up);
            pm.Transform.rotation = Quaternion.Lerp(pm.Transform.rotation, targetRotation, 0.2f);
        }
    }

    public void SetPlayerManagement(PlayerManagement management) 
    {
        pm = management;
    }
}
