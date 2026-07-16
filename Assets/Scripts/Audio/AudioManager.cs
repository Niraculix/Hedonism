using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Kampf")]
    public AudioClip attackSound;
    public AudioClip parrySound;
    public AudioClip BulletHitSound;
    public AudioClip BulletShootSound;
    public AudioClip EnemyDamageSound;
    public AudioClip EnemyDeathSound;
    public AudioClip NewWaveSound;
    public AudioClip PlayerDMGSound;
    public AudioClip EnterParryRangeSound;

    [Header("Bewegung")]
    public AudioClip jumpSound;
    public AudioClip jumpInAirSound;
    public AudioClip dashSound;
    public AudioClip landSound;
    public AudioClip footstepSound;
    public AudioClip EnterAdrenalinSound;
    public AudioClip ExitAdrenalinSound;
    public AudioClip RegainDashSound;

    [Header("Environment")]
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    public AudioClip ItemPickupSound;

    [Header("Music")]
    public AudioClip SoundTrack;

    [Header("UI")]
    public AudioClip gameOverSound;
    public AudioClip gameStartSound;
    public AudioClip UISelectSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float SFXVolume = 0.5f;

    private AudioSource audioSource;
    private AudioSource musicSource;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        StartMusic();
    }

    public void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        print($"Playing Sound: {clip.name}");
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume * SFXVolume);
    }

    public void StartMusic()
    {
        musicSource.clip = SoundTrack;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void FixedUpdate()
    {
        musicSource.volume = musicVolume;
    }
}