using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private float _time;
    private bool _inGame = true;
    public PlayerMovement PlayerMovement => playerMovement;
    public Overlaytwo Overlay => overlay;
    public ThirdPersonCam ThirdPersonCam => thirdPersonCam;
    public SpriteManagment SpriteManagment => spriteManagment;
    
    public float GameTime => _time;
    
    
    private void Start()
    {
        _time = 0;
    }

    private void Update()
    {
        if (_inGame)
        {
            _time += Time.deltaTime;
        }
    }

    public void StopTimer()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (PlayerPrefs.GetFloat(currentSceneName, -1f) < _time)
        {
            PlayerPrefs.SetFloat(currentSceneName,Mathf.Round(_time * 100) / 100);
        }
        _inGame = false;
        overlay.DisplayWinnigBanner();
    }
}
