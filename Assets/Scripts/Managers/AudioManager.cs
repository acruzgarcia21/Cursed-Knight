using CursedKnight;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;

    [Header("Card Effects")]
    [SerializeField] private AudioClip cardHoverSound;
    [SerializeField] private AudioClip attackCardPlaySound;
    [SerializeField] private AudioClip blockCardPlaySound;
    [SerializeField] private AudioClip powerCardPlaySound;

    [Space(10)] [Header("UI Effects")] 
    [Header("Button Sounds")]
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Start/End Turn Sounds")]
    [SerializeField] private AudioClip startTurnSound;
    [SerializeField] private AudioClip endTurnSound;

    [Header("Screen Sounds")]
    [SerializeField] private AudioClip cardViewOpenSound;
    [SerializeField] private AudioClip cardViewCloseSound;
    [SerializeField] private AudioClip mapOpenSound;
    [SerializeField] private AudioClip mapCloseSound;

    [Header("Battle Music")]
    [SerializeField] private AudioClip actOneBattleMusic;
    [SerializeField] private AudioClip actTwoBattleMusic;
    [SerializeField] private AudioClip actThreeBattleMusic;
    [SerializeField] private AudioClip bossBattleMusic;
    [SerializeField] private AudioClip finalBossBattleMusic;

    [Header("Other Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;
    [SerializeField] private float battleMusicVolume = 0.3f;

    private AudioSource _musicSource;

    private void Awake()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
        _musicSource.spatialBlend = 0f;
        _musicSource.volume = battleMusicVolume;
    }

    public void PlayCardHoverSound()
    {
        sfxSource.PlayOneShot(cardHoverSound);
    }

    public void PlayCardPlaySound(Card.CardType cardType)
    {
        switch (cardType)
        {
            case Card.CardType.Attack:
                sfxSource.PlayOneShot(attackCardPlaySound);
                break;
            case Card.CardType.Defense:
                sfxSource.PlayOneShot(blockCardPlaySound);
                break;
            case Card.CardType.Power:
            case Card.CardType.Utility:
                sfxSource.PlayOneShot(powerCardPlaySound);
                break;
        }
    }

    public void PlayButtonClickedSound()
    {
        sfxSource.PlayOneShot(buttonClickSound);
    }

    public void PlayStartTurnSound()
    {
        sfxSource.PlayOneShot(startTurnSound);
    }

    public void PlayEndTurnSound()
    {
        sfxSource.PlayOneShot(endTurnSound);
    }
    
    public void PlayCardViewOpenSound()
    {
        sfxSource.PlayOneShot(cardViewOpenSound);
    }

    public void PlayCardViewCloseSound()
    {
        sfxSource.PlayOneShot(cardViewCloseSound);
    }

    public void PlayMapOpenSound()
    {
        sfxSource.PlayOneShot(mapOpenSound);
    }

    public void PlayMapCloseSound()
    {
        sfxSource.PlayOneShot(mapCloseSound);
    }

    public void PlayActOneBattleMusic()
    {
        PlayMusic(actOneBattleMusic, true);
    }

    public void PlayActTwoBattleMusic()
    {
        PlayMusic(actTwoBattleMusic, true);
    }

    public void PlayActThreeBattleMusic()
    {
        PlayMusic(actThreeBattleMusic, true);
    }

    public void PlayBossBattleMusic()
    {
        PlayMusic(bossBattleMusic, true);
    }

    public void PlayFinalBossBattleMusic()
    {
        PlayMusic(finalBossBattleMusic, true);
    }

    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuMusic, true);
    }

    public void PlayVictoryMusic()
    {
        PlayMusic(victoryMusic, true);
    }

    public void PlayDefeatMusic()
    {
        PlayMusic(defeatMusic, false);
    }

    private void PlayMusic(AudioClip music, bool loop)
    {
        _musicSource.Stop();
        if (music == null) return;

        _musicSource.clip = music;
        _musicSource.loop = loop;
        _musicSource.volume = battleMusicVolume;
        _musicSource.Play();
    }

    public void StopBattleMusic()
    {
        _musicSource.Stop();
    }

    public void SetMuted(bool isMuted)
    {
        AudioListener.pause = isMuted;
    }
}