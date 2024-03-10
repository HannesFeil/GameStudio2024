using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ThirdPersonCam;

public class SpriteManagment : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameManagementRefactored gameManagement;

    [SerializeField]
    private Transform orientation;

    [SerializeField]
    private float rotationSpeed = 7f;

    [SerializeField]
    private GameObject[] animalSprites = new GameObject[4];

    private AnimalType _lastAnimal;

    private void Start()
    {
        AnimalType currentAnimal = gameManagement.PlayerMovement.GetAnimalTyp();
        _lastAnimal = currentAnimal;
        for (int i = 0; i < animalSprites.Length; i++)
        {
            animalSprites[i].SetActive((int) currentAnimal == i);
        }
    }

    private void Update()
    {
        RotatePlayer();
        SetSprite();
    }

    
    private void RotatePlayer()
    {
        if (gameManagement.ThirdPersonCam.GetcameraStyle() == CameraStyle.FREECAM || gameManagement.ThirdPersonCam.GetcameraStyle() == CameraStyle.TOPDOWN)
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
        else if (gameManagement.ThirdPersonCam.GetcameraStyle() == CameraStyle.FOCUSCAM)
        {
            transform.forward = orientation.forward;
        }
    }

    private void SetSprite()
    {
        AnimalType currentAnimal = gameManagement.PlayerMovement.GetAnimalTyp();
        if (_lastAnimal == currentAnimal) return;

        animalSprites[(int) _lastAnimal].SetActive(false);
        animalSprites[(int) currentAnimal].SetActive(true);
        _lastAnimal = currentAnimal;
    }
}
