using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsDisplay : MonoBehaviour
{
    [Header("Master Volume")] [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField] private TMP_Text masterVolumeText;

    [Space(10)] [Header("Music Volume")] [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField] private TMP_Text musicVolumeText;

    [Space(10)] [Header("Sound Effects Volume")] [SerializeField]
    private Slider soundEffectsVolumeSlider;

    [SerializeField] private TMP_Text soundEffectsVolumeText;

    [Space(10)] [Header("Mute Audio")] [SerializeField]
    private Toggle muteAudioToggle;

    private OptionsManager _optionsManager;

    private void Awake()
    {
        _optionsManager = FindAnyObjectByType<OptionsManager>();
    }

    public void OnMasterVolumeChanged(float volume)
    {
        _optionsManager.SetMasterVolume(volume);
        RefreshDisplay();
    }

    public void OnMusicVolumeChanged(float volume)
    {
        _optionsManager.SetMusicVolume(volume);
        RefreshDisplay();
    }

    public void OnSoundEffectsVolumeChanged(float volume)
    {
        _optionsManager.SetSoundEffectsVolume(volume);
        RefreshDisplay();
    }

    public void OnMuteAudioChanged(bool isMuted)
    {
        _optionsManager.SetMutingAudio(isMuted);
        RefreshDisplay();
    }

    public void ResetToDefaults()
    {
        _optionsManager.SetMasterVolume(1f);
        _optionsManager.SetMusicVolume(1f);
        _optionsManager.SetSoundEffectsVolume(1f);
        _optionsManager.SetMutingAudio(false);
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        var masterVolume       = _optionsManager.GetMasterVolume();
        var musicVolume        = _optionsManager.GetMusicVolume();
        var soundEffectsVolume = _optionsManager.GetSoundEffectsVolume();
        
        masterVolumeSlider.SetValueWithoutNotify(masterVolume);
        
        musicVolumeSlider.SetValueWithoutNotify(musicVolume);
        
        soundEffectsVolumeSlider.SetValueWithoutNotify(soundEffectsVolume);

        const float maxVolume = 100f;

        masterVolumeText.text       = Mathf.RoundToInt(masterVolume * maxVolume) + "%";
        musicVolumeText.text        = Mathf.RoundToInt(musicVolume * maxVolume) + "%";
        soundEffectsVolumeText.text = Mathf.RoundToInt(soundEffectsVolume * maxVolume) + "%";
        
        muteAudioToggle.SetIsOnWithoutNotify(_optionsManager.GetMutingAudio());
    }

}
