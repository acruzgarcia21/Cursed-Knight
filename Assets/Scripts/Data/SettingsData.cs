using System;
using UnityEngine;

[Serializable]
public class SettingsData
{
    [SerializeField] private bool mutingAudio;

    public SettingsData(bool mutingAudio)
    {
        this.mutingAudio = mutingAudio;
    }

    public bool GetMutingAudio()
    {
        return mutingAudio;
    }
}