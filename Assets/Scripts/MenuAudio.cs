using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip menuMusic;

    [Header("UI Sound")]
    public AudioSource uiSource;
    public AudioClip buttonHoverClip;
    public AudioClip buttonClickClip;

    [Header("Settings")]
    public float musicVolume = 0.3f;

    public static MenuAudio Instance;

    void Start()
    {
        if (musicSource != null && menuMusic != null)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void OnButtonHover()
    {
        if (uiSource != null && buttonHoverClip != null)
            uiSource.PlayOneShot(buttonHoverClip, 0.4f);
    }

    public void OnButtonClick()
    {
        if (uiSource != null && buttonClickClip != null)
            uiSource.PlayOneShot(buttonClickClip, 0.6f);
    }

    void Awake()
    {
        Instance = this;
    }
}