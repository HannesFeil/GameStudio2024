using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ThirdPersonCam;

public class SpriteManagment : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private PlayerMovement _pm;
    [SerializeField]
    private ThirdPersonCam _th;

    [SerializeField]
    private float rotationSpeed = 7f;

    [SerializeField]
    private GameObject[] animalSprites = new GameObject[4];

    private AnimalType _lastAnimal;

    private void Update()
    {
        RotatePlayer();
        SetSprite();
    }

    private void RotatePlayer()
    {
        if (_th.GetcameraStyle() == CameraStyle.FREECAM || _th.GetcameraStyle() == CameraStyle.TOPDOWN)
        {
            // rotate player object
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
            if (inputDir != Vector3.zero)
            {
                transform.forward = Vector3.Slerp(transform.forward, inputDir.normalized, rotationSpeed * Time.deltaTime);
            }
        }
        else if (_th.GetcameraStyle() == CameraStyle.FOCUSCAM)
        {
            transform.forward = orientation.forward;
        }
    }

    private void SetSprite()
    {
        AnimalType currentAnimal = _pm.GetAnimalTyp();
        if (_lastAnimal == currentAnimal) return;

        animalSprites[(int) _lastAnimal].SetActive(false);
        animalSprites[(int) currentAnimal].SetActive(true);
        _lastAnimal = currentAnimal;
    }
}
