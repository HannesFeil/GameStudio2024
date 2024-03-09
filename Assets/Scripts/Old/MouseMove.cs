using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : AnimalMove
{
    private bool _charge = false;
    private float _force = 0;
    [SerializeField] float forceIncrease = 5f;
    [SerializeField] float staminaDrain = 5f;
    public override void Setup()
    {
        print("MOUSE");
    }

    public override void Move(Vector2 dir, bool specialActive)
    {
        pm.CamLookAtPlayer();
        CheckJump();
        UpdateRotation();
        if (specialActive && !_charge && stamina > 0)
        {
            _charge = true;
            _force = forceIncrease;
        }
        else if(specialActive && stamina > 0)
        {
            _force += forceIncrease;
            //stamina -= staminaDrain;
        }
        else
        {
           
            _charge = false;
            
            pm.GetRigidbody().AddForce(pm.GetCamDir().normalized * _force);
            _force = 0;
        }
        
        if (!pm.IsGrounded()) {
            dir *= pm.AirMovementFactor;
        }
        
        pm.GetRigidbody().AddForce(dir.x * pm.MovementForce, 0, dir.y * pm.MovementForce);
        pm.CheckSwap();
    }
}
