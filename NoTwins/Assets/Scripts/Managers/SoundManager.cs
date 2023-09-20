using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private static bool isSoundPlaying;

    [Header("Audio Sources"), SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundsSource;

    [Header("Audio Clips")]
    public AudioClip buttonClickSound;
    public AudioClip matchedSound;
    public AudioClip mismatchedSound;
    public AudioClip gameCompleteSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        CheckAudio();
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayMusic()
    {
        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    public void PlaySoundAtPosition(AudioClip clip, Vector3 pos, float vol = 1)
    {
        soundsSource.transform.position = pos;
        PlaySound(clip, vol);
    }

    public void PlaySound(AudioClip clip, float vol = 1)
    {
        soundsSource.PlayOneShot(clip, vol);
    }

    public void SoundToggle(bool isSoundOn)
    {
        isSoundPlaying = isSoundOn;
        CheckAudio();
    }

    private static void CheckAudio()
    {
        AudioListener.volume = (isSoundPlaying) ? 0 : 1;
    }
}
