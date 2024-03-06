using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalMove : MonoBehaviour
{
    protected PlayerManagement pm;

    public void Move(Vector2 dir, bool specialActive) {
        if (!pm.IsGrounded()) {
            dir *= pm.airMovementFactor;
        }
        pm.rb.AddForce(dir.x * pm.movementForce, 0, dir.y * pm.movementForce);
    }

    public void Jump() {
        pm.rb.AddForce(0, pm.jumpForce, 0);
    }

    public void SetPlayerManagement(PlayerManagement management) {
        pm = management;
    }
}
