using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private PauseMenuDisplay pauseMenuDisplay;

    private bool _isGamePaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isGamePaused) ResumeGame();
            else PauseGame();
        }
    }

    private void Start()
    {
        ResumeGame();
    }

    public bool GetIsGamePaused()
    {
        return _isGamePaused;
    }
    

    public void PauseGame()
    {
        _isGamePaused = true;

        Time.timeScale = 0;
        
        pauseMenuDisplay.ShowPauseScreen();
    }

    public void ResumeGame()
    {
        _isGamePaused = false;

        Time.timeScale = 1;
        
        pauseMenuDisplay.HidePauseScreen();
    }
}
