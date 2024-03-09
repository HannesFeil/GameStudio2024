using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrientation : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform wallDirection;
    [SerializeField]
    private PlayerMovement _pm;

    [SerializeField]
    private float rotationSpeed = 7f;

    private void Update()
    {
        // rotate player object
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (!_pm.GetWallrunning())
        {
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
            if (inputDir != Vector3.zero)
            {
                transform.forward = Vector3.Slerp(transform.forward, inputDir.normalized, rotationSpeed * Time.deltaTime);
            }
        } else
        {
            transform.up = wallDirection.up;
        }
        
    }
}
