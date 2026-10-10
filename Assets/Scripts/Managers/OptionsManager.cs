using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    private AudioManager _audioManager;
    
    private string _settingsSavePath;

    // Volume Settings Attributes
    private bool _mutingAudio;
    
    private float _masterVolume       = 1f;
    private float _musicVolume        = 1f;
    private float _soundEffectsVolume = 1f;
    
    // Graphics Settings Attributes
    private bool _isFullscreen = true;
    private bool _vSync = true;

    private int _fpsLimit = 60;

    private int _windowWidth = 1280;
    private int _windowHeight = 720;
    
    // Font Settings Attributes
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
    
    // =========================================================
    // Graphics Settings
    // =========================================================

    public void SetFPSLimit(int fpsLimit)
    {
        _fpsLimit = fpsLimit;

        Application.targetFrameRate = _fpsLimit;
    }

    public int GetFPSLimit()
    {
        return _fpsLimit;
    }

    public void SetWindowedResolution(int width, int height)
    {
        _windowWidth  = width;
        _windowHeight = height;

        if (_isFullscreen) return;
        
        Screen.SetResolution(_windowWidth, _windowHeight, FullScreenMode.Windowed);
    }

    public int GetWindowedWidth()
    {
        return _windowWidth;
    }

    public int GetWindowedHeight()
    {
        return _windowHeight;
    }
    
    public void SetFullScreen(bool isFullscreen)
    {
        _isFullscreen = isFullscreen;

        Screen.fullScreenMode = _isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        
        if (!isFullscreen) SetWindowedResolution(_windowWidth, _windowHeight);
    }

    public void SetVSync(bool isEnabled)
    {
        _vSync = isEnabled;

        QualitySettings.vSyncCount = _vSync ? 1 : 0;
    }

    public bool GetVSync()
    {
        return _vSync;
    }

    public bool GetFullScreen()
    {
        return _isFullscreen;
    }
    
    // =========================================================
    // Volume Settings
    // =========================================================

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
    
    public void SetMutingAudio(bool isMuted)
    {
        _mutingAudio = isMuted;
        _audioManager.SetMuted(_mutingAudio);
    }

    public bool GetMutingAudio()
    {
        return _mutingAudio;
    }
    
    // =========================================================
    // Settings Persistence
    // =========================================================

    public void RestoreSettings(SettingsData settingsData)
    {
        SetMasterVolume(settingsData.GetMasterVolume());
        SetMusicVolume(settingsData.GetMusicVolume());
        SetSoundEffectsVolume(settingsData.GetSoundEffectsVolume());
        SetMutingAudio(settingsData.GetMutingAudio());

        SetWindowedResolution(settingsData.GetWindowedWidth(), settingsData.GetWindowedHeight());
        SetFullScreen(settingsData.GetFullScreen());
        SetVSync(settingsData.GetVSync());
        SetFPSLimit(settingsData.GetFPSLimit());
    }
    
    public void SaveSettings()
    {
        var settingsData = new SettingsData(GetMutingAudio(), GetMasterVolume(), GetMusicVolume(), GetSoundEffectsVolume(), GetFullScreen(), GetVSync(), GetFPSLimit(), GetWindowedWidth(), GetWindowedHeight());

        var json = JsonUtility.ToJson(settingsData, true);

        File.WriteAllText(_settingsSavePath, json);

        Debug.Log($"Settings saved to: {_settingsSavePath}");
    }

    public void LoadSettings()
    {
        var settingsData = new SettingsData(false, 1f, 1f, 
            1f, true, true, 60, 1280, 720);

        if (!File.Exists(_settingsSavePath))
        {
            RestoreSettings(settingsData);
            return;
        }

        var json = File.ReadAllText(_settingsSavePath);

        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "null")
        {
            Debug.LogWarning("OptionsManager: Failed to load settings.");
            return;
        }

        // Missing fields in older settings files keep their defaults.
        JsonUtility.FromJsonOverwrite(json, settingsData);

        RestoreSettings(settingsData);
    }
    
    // =========================================================
    // Font Settings
    // =========================================================

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