using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public List<TMP_FontAsset> fontList;
    public static event Action FontUpdated;
    
    private AudioManager _audioManager;

    private bool _mutingAudio;
    
    private string _settingsSavePath;

    [Header("Volume Settings")] 
    private float _masterVolume       = 1f;
    private float _musicVolume        = 1f;
    private float _soundEffectsVolume = 1f;

    private void Awake()
    {
        _audioManager = FindAnyObjectByType<AudioManager>();
        
        _settingsSavePath = Path.Combine(Application.persistentDataPath, "settings.json");
    }

    private void Start()
    {
        LoadSettings();
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        _audioManager.SetMasterVolume(_masterVolume);
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        _audioManager.SetMusicVolume(_musicVolume);
    }

    public void SetSoundEffectsVolume(float volume)
    {
        _soundEffectsVolume = Mathf.Clamp01(volume);
        _audioManager.SetSoundEffectsVolume(_soundEffectsVolume);
    }

    public float GetMasterVolume()
    {
        return _masterVolume;
    }

    public float GetMusicVolume()
    {
        return _musicVolume;
    }

    public float GetSoundEffectsVolume()
    {
        return _soundEffectsVolume;
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
        SetMasterVolume(settingsData.GetMasterVolume());
        SetMusicVolume(settingsData.GetMusicVolume());
        SetSoundEffectsVolume(settingsData.GetSoundEffectsVolume());
        SetMutingAudio(settingsData.GetMutingAudio());
    }
    
    public void SaveSettings()
    {
        var settingsData = new SettingsData(GetMutingAudio(), GetMasterVolume(), GetMusicVolume(), GetSoundEffectsVolume());

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

        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "null")
        {
            Debug.LogWarning("OptionsManager: Failed to load settings.");
            return;
        }

        // Older settings files only contain mute, so missing volumes keep their defaults.
        var settingsData = new SettingsData(false, 1f, 1f, 1f);
        JsonUtility.FromJsonOverwrite(json, settingsData);

        RestoreSettings(settingsData);
    }
}