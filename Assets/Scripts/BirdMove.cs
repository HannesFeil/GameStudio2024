using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMove : AnimalMove
{
    private float _groundMass = 1f;
    private int flyTimer;
    
    [SerializeField]
    [Range(0,1)]
    private float airMass = 0.5f;

    private Rigidbody rb;
    public override void Setup()
    {
        rb = pm.GetRigidbody();
    }

    void FixedUpdate()
    {
        
    }
    
    public override void Move(Vector2 dir, bool specialActive) 
    {
        pm.CamLookAtPlayer();
        CheckJump();
        UpdateRotation();

        if (!pm.IsGrounded())
        {
            dir *= pm.AirMovementFactor;
        }

        if (specialActive)
        {
            Vector3 velo = rb.velocity;
            CheckJump();
            //pm.GetRigidbody().mass = airMass; //- (flyTimer * 0.1f);
            rb.velocity = new Vector3(velo.x, Mathf.Max(velo.y, -0.1f), velo.z);
        }
        
        if (pm.IsGrounded())
        {
            pm.GetRigidbody().mass = _groundMass;
        }
        pm.GetRigidbody().AddForce(dir.x * pm.MovementForce, 0, dir.y * pm.MovementForce);
        pm.CheckSwap();
    }
    
    /// <summary>
    /// Get"s called after this animal is swapped away
    /// </summary>
    public override void OnSwappedFrom() {
        pm.GetRigidbody().mass = _groundMass;
    }

    /// <summary>
    /// Get"s called after this animal is swapped to
    /// </summary>
    public override void OnSwappedTo() {
        
    }

    
    // public void CheckJump() 
    // {
    //     if (Input.GetButton("Jump") && pm.IsGrounded()) {
    //         Jump();
    //         pm.SetNotGrounded();
    //     }
    // }
    //
    // public void Jump()
    // {
    //     pm.GetRigidbody().AddForce(0, pm.JumpForce, 0);
    // }
    //
    // public void Glide()
    // {
    //     
    // }
    //
    // public void UpdateRotation() 
    // {
    //     Quaternion targetRotation;
    //     if (pm.GetRigidbody().velocity.magnitude > 0.5) {
    //         targetRotation = Quaternion.LookRotation(pm.GetRigidbody().velocity, Vector3.up);
    //     } else {
    //         targetRotation = Quaternion.Euler(0, pm.transform.rotation.eulerAngles.y, 0);
    //     }
    //     pm.GetTransform().rotation = Quaternion.Lerp(pm.GetTransform().rotation, targetRotation, 0.2f);
    // }
}
