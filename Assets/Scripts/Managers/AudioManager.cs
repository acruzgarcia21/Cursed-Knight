using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public void SetMuted(bool isMuted)
    {
        AudioListener.pause = isMuted;
    }
}