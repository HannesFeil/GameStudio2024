using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalMove : MonoBehaviour
{
    public abstract void Move(Vector2 dir, bool specialActive);

    public abstract void Jump();

    public abstract void SetRb(Rigidbody rb);
}
