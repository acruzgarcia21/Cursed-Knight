using System;
using UnityEngine;

[Serializable]
public class SettingsData
{
    [Header("Volume Settings Data")]
    [SerializeField] private float masterVolume;
    [SerializeField] private float musicVolume;
    [SerializeField] private float soundEffectsVolume;

    [SerializeField] private bool mutingAudio;
    
    public SettingsData(bool mutingAudio, float masterVolume, float musicVolume, float soundEffectsVolume)
    {
        this.mutingAudio = mutingAudio;
        this.masterVolume = masterVolume;
        this.musicVolume = musicVolume;
        this.soundEffectsVolume = soundEffectsVolume;
    }

    public bool GetMutingAudio()
    {
        return mutingAudio;
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSoundEffectsVolume()
    {
        return soundEffectsVolume;
    }
}
