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

    private bool _cancelJumping = false;

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
        SetAnimation();
        RotateLizzard();
    }

    
    private void RotatePlayer()
    {
        if(gameManagement.PlayerMovement.getMoveState() == PlayerMovement.MovementStat.climbing) return;
        if (gameManagement.ThirdPersonCam.GetcameraStyle() == CameraStyle.Free || gameManagement.ThirdPersonCam.GetcameraStyle() == CameraStyle.Topdown)
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
        else if (gameManagement.ThirdPersonCam.GetcameraStyle() == CameraStyle.Focus)
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

    private void SetNotJumping() {
        gameManagement.PlayerMovement.SetNotJumping();
        _cancelJumping = false;
    }

    private void SetAnimation()
    {
        PlayerMovement.MovementStat movementStat = gameManagement.PlayerMovement.getMoveState();
        AnimalType currentAnimal = gameManagement.PlayerMovement.GetAnimalTyp();
        Animator animator = animalSprites[(int)currentAnimal].GetComponent<Animator>();

        switch (movementStat)
        {
            case PlayerMovement.MovementStat.air: 
                animator.Play("Eyes_Shrink");
                if (gameManagement.PlayerMovement.isJumping()) {
                    animator.Play("Jump");
                    if (!_cancelJumping) {
                        Invoke(nameof(SetNotJumping), 0.4f);
                        _cancelJumping = true;
                    }
                } else {
                    animator.Play("Swim");
                }
                break;
            case PlayerMovement.MovementStat.climbing: 
                animator.Play("Walk");
                break;
            case PlayerMovement.MovementStat.dashing: 
                //Wenn die Maus ihren Dash macht, macht sie Liegestütze und klatscht
                animator.Play("Bounce");
                animator.Play("Eyes_Blink");
                break;
            case PlayerMovement.MovementStat.gliding: 
                //Wenn der Vogel von etwas herunter gleitet, fliegt er
                animator.Play("Fly");
                break;
            case PlayerMovement.MovementStat.swinging: 
                //Wenn die Schlange am Enterhaken schwingt, dreht sie sich
                animator.Play("Spin");
                break;
            case PlayerMovement.MovementStat.grappling:
                //Wenn die Schlange den Enterhaken schwingt, macht sie eine Attacke
                animator.Play("Attack");
                break;
            case PlayerMovement.MovementStat.walking:
                //Wenn die Tiere laufen, dann laufen sie
                animator.Play("Walk");
                animator.Play("Eyes_Excited");
                break;
        }
        //animator.Play("Spin");
    }

    private void RotateLizzard()
    {
        PlayerMovement.MovementStat movementStat = gameManagement.PlayerMovement.getMoveState();
        if (movementStat != PlayerMovement.MovementStat.climbing) return;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(-horizontalInput, 0, verticalInput);
        if (inputDir != Vector3.zero) {
            Vector3 wallNormal = gameManagement.PlayerMovement.GetLastWallNormal();
            Quaternion wallRotation = Quaternion.LookRotation(wallNormal, Vector3.up);
            Vector3 forward = Quaternion.Euler(wallRotation.eulerAngles.x - 90, wallRotation.eulerAngles.y, 0) * inputDir;
            transform.rotation = Quaternion.LookRotation(forward, wallNormal);
        }
    }
}
