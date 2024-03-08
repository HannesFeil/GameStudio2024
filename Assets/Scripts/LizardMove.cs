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
    
        if (!pm.IsGrounded()) {
            dir *= pm.AirMovementFactor;
        }

        Vector3 forceDir = new Vector3(dir.x * pm.MovementForce, 0, dir.y * pm.MovementForce);

        bool onWall = false;
        RaycastHit hit = new RaycastHit();
        if (specialActive && stamina > 0) {
            float fwdRatio = Vector3.Angle(pm.GetTransform().up, Vector3.up) / 90;
            Vector3 checkDir = Vector3.Lerp(pm.GetTransform().forward, -pm.GetTransform().up, fwdRatio);

            if (Physics.Raycast(pm.GetTransform().position, checkDir, out hit, 1)) {
                onWall = true;
            }
        }
        
        if (onWall) {
            stamina -= 0.1f;
            print(stamina);
            pm.GetRigidbody().useGravity = false;
            UpdateWallRotation(hit.normal);
            pm.GetRigidbody().AddForce(
                Quaternion.AngleAxis(90, Vector3.Cross(-pm.GetTransform().up, Vector3.up))
                * forceDir
                + -pm.GetTransform().up * 0.1f
            );
        } else {
            pm.GetRigidbody().useGravity = true;
            UpdateRotation();
            pm.GetRigidbody().AddForce(forceDir);
        }
                
        pm.CheckSwap();
    }
    
    public void UpdateWallRotation(Vector3 normal) 
    {
        Quaternion targetRotation;
        if (pm.GetRigidbody().velocity.magnitude > 0.5) {
            targetRotation = Quaternion.LookRotation(pm.GetRigidbody().velocity, normal);
        } else {
            targetRotation = Quaternion.Euler(-90, pm.GetTransform().eulerAngles.y, pm.GetTransform().rotation.eulerAngles.z);
        }
        pm.GetTransform().rotation = Quaternion.Lerp(pm.GetTransform().rotation, targetRotation, 0.2f);
    }

    public override void OnSwappedFrom()
    {
        pm.GetRigidbody().useGravity = true;
    }
}
