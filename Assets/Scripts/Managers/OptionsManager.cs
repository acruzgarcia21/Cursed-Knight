using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    private AudioManager _audioManager;

    [SerializeField] private bool mutingAudio = false;

    public List<TMP_FontAsset> fontList;
    public static event Action FontUpdated;

    private void Awake()
    {
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

    public void SetMutingAudio(bool isMuted)
    {
        mutingAudio = isMuted;
        _audioManager.SetMuted(mutingAudio);
    }

    public bool GetMutingAudio()
    {
        return mutingAudio;
    }

    public void RestoreSettings(SettingsData settingsData)
    {
        SetMutingAudio(settingsData.GetMutingAudio());
    }
}