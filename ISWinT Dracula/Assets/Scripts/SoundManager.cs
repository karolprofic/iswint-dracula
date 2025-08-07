using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clip")]
    public AudioClip backgroundMusic;

    [Header("SFX Clips")]
    public AudioClip walkClip;
    public AudioClip jumpClip;
    public AudioClip deadClip;
    public AudioClip takingDamageClip;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayWalkSound()
    {
        PlaySFX(walkClip);
    }

    public void PlayJumpSound()
    {
        PlaySFX(jumpClip);
    }

    public void PlayDeadSound()
    {
        PlaySFX(deadClip);
    }

    public void PlayTakingDamageSound()
    {
        PlaySFX(takingDamageClip);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
