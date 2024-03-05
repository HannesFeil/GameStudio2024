using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardMove : AnimalMove
{
    private Rigidbody _rb;
    public override void Move(Vector2 dir, bool specialActive)
    {
        _rb.AddForce(dir.x * 10,0,dir.y * 10);
    }
    
    
    public override void Jump()
    {
        throw new System.NotImplementedException();
    }

    public override void SetRb(Rigidbody rb)
    {
        _rb = rb;
    }
}
