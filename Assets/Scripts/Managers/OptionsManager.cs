using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    private AudioManager _audioManager;

    private bool _mutingAudio;
    
    private string _settingsSavePath;

    public List<TMP_FontAsset> fontList;
    public static event Action FontUpdated;

    private void Awake()
    {
        _audioManager = FindAnyObjectByType<AudioManager>();
        
        _settingsSavePath = Path.Combine(Application.persistentDataPath, "settings.json");
    }

    private void Start()
    {
        LoadSettings();
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
    
    public void SaveSettings()
    {
        var settingsData = new SettingsData(GetMutingAudio());

        var json = JsonUtility.ToJson(settingsData, true);

        File.WriteAllText(_settingsSavePath, json);

        Debug.Log($"Settings saved to: {_settingsSavePath}");
    }

    public void LoadSettings()
    {
        if (!File.Exists(_settingsSavePath))
        {
            return;
        }

        var json = File.ReadAllText(_settingsSavePath);

        var settingsData = JsonUtility.FromJson<SettingsData>(json);

        if (settingsData == null)
        {
            Debug.LogWarning("OptionsManager: Failed to load settings.");
            return;
        }

        RestoreSettings(settingsData);
    }
}