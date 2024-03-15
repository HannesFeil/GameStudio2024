using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagment : MonoBehaviour
{
    [Header("Reverences")]
    [SerializeField] 
    private Canvas menu;

    [SerializeField] private GameManagementRefactored gameManagement;
    
    
    
    [Header("keybinds")] 
    [SerializeField] 
    private KeyCode pause = KeyCode.Escape;

    [SerializeField] 
    private KeyCode restart = KeyCode.R;

    private bool _paused;
    
    // Start is called before the first frame update
    void Start()
    {
        menu.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pause))
        {
            if (!gameManagement.InGame)
            {
                ToMainMenue();
            }
            else if (_paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (Input.GetKeyDown(restart))
        {
            Reload();
        }
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _paused = false;
        menu.enabled = false;
        Time.timeScale = 1;
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _paused = true;
        menu.enabled = true;
        Time.timeScale = 0;
    }

    public void ToMainMenue()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Reload()
    {
        Time.timeScale = 1;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
