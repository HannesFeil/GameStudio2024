using UnityEngine;

public class LizardMove : AnimalMove
{
    [SerializeField]
    bool infiniteStamina = false;
    
    [SerializeField]
    float wallJumpFactor = 2f;

    private float _wallJumpTimer = 0;
    private float _wallJumpTimerMax = 10;
    
    /// <summary>
    /// Main tick method of each animal
    /// </summary>
    public override void Move(Vector2 dir, bool specialActive) 
    {
        _wallJumpTimer = Mathf.Max(0, _wallJumpTimer - 1);
        pm.CamLookAtPlayer();

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
            if (!infiniteStamina) {
                stamina -= 0.1f;
            }
            
            pm.GetRigidbody().useGravity = false;
            UpdateWallRotation(hit.normal);
            pm.GetRigidbody().AddForce(
                Quaternion.FromToRotation(Vector3.up, hit.normal)
                * forceDir
                + -pm.GetTransform().up * 0.1f
            );
            if (Input.GetButton("Jump") && _wallJumpTimer == 0) {
                _wallJumpTimer = _wallJumpTimerMax;
                pm.GetRigidbody().AddForce(Vector3.Lerp(Vector3.up, hit.normal, 0.5f) * pm.JumpForce * wallJumpFactor);
            }
        } else {
            pm.GetRigidbody().useGravity = true;
            UpdateRotation();
            if (!pm.IsGrounded()) {
                dir *= pm.AirMovementFactor;
            }
            pm.GetRigidbody().AddForce(forceDir);
            CheckJump();
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
