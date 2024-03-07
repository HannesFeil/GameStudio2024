using UnityEngine;

public class LizardMove : AnimalMove
{
    /// <summary>
    /// Main tick method of each animal
    /// </summary>
    public override void Move(Vector2 dir, bool specialActive) 
    {
        pm.CamLookAtPlayer();
        CheckJump();
        UpdateRotation();
    
        if (!pm.IsGrounded()) {
            dir *= pm.AirMovementFactor;
        }
        
        pm.GetRigidbody().AddForce(dir.x * pm.MovementForce, 0, dir.y * pm.MovementForce);
        pm.CheckSwap();
    }
}
