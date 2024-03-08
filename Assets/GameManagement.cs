using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    private static GameManagement _instance;

    private PlayerManagement _playerManagement;
    private Overlay _overlay;
    
    public void Awake()
    {
        _instance = this;
        _playerManagement = GameObject.Find("Player").GetComponent<PlayerManagement>();
        _overlay = GameObject.Find("Overlay").GetComponent<Overlay>();
    }

    public static GameManagement GetInstance() 
    {
        return _instance;    
    }

    public PlayerManagement GetPlayerManagement() 
    {
        return _playerManagement;
    }

    public Overlay GetOverlay()
    {
        return _overlay;
    }
}
