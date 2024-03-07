using UnityEngine;

public abstract class AnimalMove : MonoBehaviour
{
    protected PlayerManagement pm;

    public virtual void Move(Vector2 dir, bool specialActive) 
    {
        pm.CamLookAtPlayer();
        CheckJump();
        UpdateRotation();
        pm.CheckSwap();
    
        if (!pm.IsGrounded()) {
            dir *= pm.AirMovementFactor;
        }
        
        pm.GetRigidbody().AddForce(dir.x * pm.MovementForce, 0, dir.y * pm.MovementForce);
    }

    public void CheckJump() 
    {
        if (Input.GetButton("Jump") && pm.IsGrounded()) {
            Jump();
            pm.SetNotGrounded();
        }
    }

    public void Jump() 
    {
        pm.GetRigidbody().AddForce(0, pm.JumpForce, 0);
    }

    public void UpdateRotation() 
    {
        Quaternion targetRotation;
        if (pm.GetRigidbody().velocity.magnitude > 0.5) {
            targetRotation = Quaternion.LookRotation(pm.GetRigidbody().velocity, Vector3.up);
        } else {
            targetRotation = Quaternion.Euler(0, pm.transform.rotation.eulerAngles.y, 0);
        }
        pm.GetTransform().rotation = Quaternion.Lerp(pm.GetTransform().rotation, targetRotation, 0.2f);
    }

    public void SetPlayerManagement(PlayerManagement management) 
    {
        pm = management;
    }
}
