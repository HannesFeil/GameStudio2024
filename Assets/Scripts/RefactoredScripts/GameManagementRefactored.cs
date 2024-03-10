using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagementRefactored : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private Overlaytwo overlay;
    [SerializeField]
    private ThirdPersonCam thirdPersonCam;
    [SerializeField]
    private SpriteManagment spriteManagment;

    public PlayerMovement PlayerMovement => playerMovement;
    public Overlaytwo Overlay => overlay;
    public ThirdPersonCam ThirdPersonCam => thirdPersonCam;
    public SpriteManagment SpriteManagment => spriteManagment;
}
