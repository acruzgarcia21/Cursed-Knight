using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private PauseMenuDisplay pauseMenuDisplay;

    private SaveManager _saveManager;

    private bool _isGamePaused;

    private void Awake()
    {
        _saveManager = FindAnyObjectByType<SaveManager>();
    }

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

    public void AbandonRun()
    {
        _saveManager.DisableRunSaving();
        _saveManager.DeleteRunSave();

        _isGamePaused = false;

        Time.timeScale = 1;
        
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    public void SaveAndQuit()
    {
        _saveManager.SaveRun();
        
        _isGamePaused = false;

        Time.timeScale = 1;
        
        Loader.Load(Loader.Scene.MainMenuScene);
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
