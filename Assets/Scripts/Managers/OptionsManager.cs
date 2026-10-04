using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    private AudioManager _audioManager;

    private bool _mutingAudio;

    public List<TMP_FontAsset> fontList;
    public static event Action FontUpdated;

    private void Awake()
    {
        _audioManager = FindAnyObjectByType<AudioManager>();
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

    public void SetMutingAudio(bool isMuted)
    {
        _mutingAudio = isMuted;
        _audioManager.SetMuted(_mutingAudio);
    }

    public bool GetMutingAudio()
    {
        return _mutingAudio;
    }

    public void RestoreSettings(SettingsData settingsData)
    {
        SetMutingAudio(settingsData.GetMutingAudio());
    }
}