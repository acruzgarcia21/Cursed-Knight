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

    [Header("Graphics Settings Data")]
    [SerializeField] private bool fullscreen;
    [SerializeField] private bool vSync;
    [SerializeField] private int fpsLimit;
    [SerializeField] private int windowedWidth;
    [SerializeField] private int windowedHeight;
    
    [SerializeField] private bool reduceMotion;
    [SerializeField] private bool confirmEndTurn;

    public SettingsData(bool mutingAudio, float masterVolume, float musicVolume, float soundEffectsVolume, 
        bool fullscreen, bool vSync, int fpsLimit, int windowedWidth, int windowedHeight, bool reduceMotion = false, bool confirmEndTurn = false)
    {
        this.mutingAudio = mutingAudio;
        this.masterVolume = masterVolume;
        this.musicVolume = musicVolume;
        this.soundEffectsVolume = soundEffectsVolume;
        this.fullscreen = fullscreen;
        this.vSync = vSync;
        this.fpsLimit = fpsLimit;
        this.windowedWidth = windowedWidth;
        this.windowedHeight = windowedHeight;
        this.reduceMotion = reduceMotion;
        this.confirmEndTurn = confirmEndTurn;
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

    public bool GetFullScreen()
    {
        return fullscreen;
    }

    public bool GetVSync()
    {
        return vSync;
    }

    public int GetFPSLimit()
    {
        return fpsLimit;
    }

    public int GetWindowedWidth()
    {
        return windowedWidth;
    }

    public int GetWindowedHeight()
    {
        return windowedHeight;
    }

    public bool GetReduceMotion()
    {
        return reduceMotion;
    }

    public bool GetConfirmEndTurn()
    {
        return confirmEndTurn;
    }
}
