using UnityEngine;

public abstract class AnimalMove : MonoBehaviour
{
    protected PlayerManagement pm;

    /// <summary>
    /// Main tick method of each animal
    /// </summary>
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

    /// <summary>
    /// Check if the animal should jump and jump
    /// </summary>
    public void CheckJump() 
    {
        if (Input.GetButton("Jump") && pm.IsGrounded()) {
            Jump();
            pm.SetNotGrounded();
        }
    }

    /// <summary>
    /// Makes the animal jump
    /// </summary>
    public void Jump() 
    {
        pm.GetRigidbody().AddForce(0, pm.JumpForce, 0);
    }

    /// <summary>
    /// Updates animal rotation according to velocity
    /// </summary>
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

    /// <summary>
    /// Set the PlayerManagement
    /// </summary>
    public void SetPlayerManagement(PlayerManagement management) 
    {
        pm = management;

        Setup();
    }

    /// <summary>
    /// Get's called after the PlayerManagement has been initialized
    /// </summary>
    public virtual void Setup() {
        
    }

    /// <summary>
    /// Get"s called after this animal is swapped away
    /// </summary>
    public virtual void OnSwappedFrom() {
        
    }

    /// <summary>
    /// Get"s called after this animal is swapped to
    /// </summary>
    public virtual void OnSwappedTo() {
        
    }
}
