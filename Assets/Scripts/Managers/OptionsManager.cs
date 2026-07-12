using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    private AudioManager _audioManager;
    
    public bool mutingAudio = false;

    public List<TMP_FontAsset> fontList;
    public static event Action FontUpdated;

    private void Start()
    {
        // Set up a reference to find the audio manager
        _audioManager = GameManager.Instance.AudioManager;
    }

    public TMP_FontAsset GetFontClass(string classID)
    {
        return classID switch
        {
            "MenuText" => fontList[0],
            "CardTitle" => fontList[1],
            "CardBody" => fontList[2],
            "CardBodyBold" => fontList[3],
            "MenuTextBold" => fontList[4],
            _ => fontList[0]
        };
    }

    public void UpdateFont()
    {
        FontUpdated?.Invoke();
    }
}
