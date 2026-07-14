using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Kampf")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip parrySound;

    [Header("Bewegung")]
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip landSound;

    [Header("UI")]
    public AudioClip gameOverSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume * masterVolume);
    }
}