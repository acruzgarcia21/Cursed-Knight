using System;
using UnityEngine;

public class GameplayStartup : MonoBehaviour
{
    private RunManager  _runManager;
    private SaveManager _saveManager;

    private void Awake()
    {
        _runManager  = FindAnyObjectByType<RunManager>();
        _saveManager = FindAnyObjectByType<SaveManager>();
    }

    private void Start()
    {
        var runStartRequest = GameManager.Instance.ConsumeRunStartRequest();

        switch (runStartRequest)
        {
            case GameManager.RunStartRequest.NewRun:
                _runManager.StartNewRun();
                break;
            case GameManager.RunStartRequest.Continue:
                _saveManager.LoadRun();
                break;
            case GameManager.RunStartRequest.None:
                break;
        }
    }
}
