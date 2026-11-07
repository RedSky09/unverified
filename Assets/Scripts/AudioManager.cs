using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Footsteps")]
    public AudioClip[] footstepClips;
    public AudioSource footstepSource;
    public float footstepInterval = 0.4f;

    [Header("Interaction")]
    public AudioSource interactionSource;
    public AudioClip drawerSlideClip;
    public AudioClip batteryPickupClip;
    public AudioClip flashlightOnClip;
    public AudioClip flashlightOffClip;

    [Header("Ambient")]
    public AudioSource ambientSource;
    public AudioClip[] ambientClips;

    [Header("Mixer")]
    public UnityEngine.Audio.AudioMixerGroup ambientMixerGroup;
    public UnityEngine.Audio.AudioMixerGroup clockMixerGroup;

    private float footstepTimer = 0f;
    private bool isMoving = false;

    void Awake()
    {
        Instance = this;

        // Pre-load semua clip supaya tidak ada delay saat pertama dipanggil
        if (flashlightOnClip != null) AudioSettings.Reset(AudioSettings.GetConfiguration());

        // Warm up audio source
        if (interactionSource != null)
        {
            interactionSource.volume = 0f;
            interactionSource.PlayOneShot(flashlightOnClip);
            interactionSource.volume = 1f;
        }
    }

    void Update()
    {
        HandleFootsteps();
    }

    void HandleFootsteps()
    {
        // Cek movement dari keyboard
        bool moving = false;
        if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed ||
            UnityEngine.InputSystem.Keyboard.current.sKey.isPressed ||
            UnityEngine.InputSystem.Keyboard.current.aKey.isPressed ||
            UnityEngine.InputSystem.Keyboard.current.dKey.isPressed)
            moving = true;

        isMoving = moving;

        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;
        if (footstepSource == null) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        footstepSource.PlayOneShot(clip, 1f);
    }

    // Dipanggil dari Flashlight.cs
    public void PlayFlashlightOn()
    {
        if (interactionSource != null && flashlightOnClip != null)
            interactionSource.PlayOneShot(flashlightOnClip, 0.7f);
    }

    public void PlayFlashlightOff()
    {
        if (interactionSource != null && flashlightOffClip != null)
            interactionSource.PlayOneShot(flashlightOffClip, 0.7f);
    }

    // Dipanggil dari Drawer.cs
    public void PlayDrawerSlide()
    {
        if (interactionSource != null && drawerSlideClip != null)
            interactionSource.PlayOneShot(drawerSlideClip, 1f);
    }

    public void PlayBatteryPickup()
    {
        if (interactionSource != null && batteryPickupClip != null)
            interactionSource.PlayOneShot(batteryPickupClip, 0.3f);
    }

    // Dipanggil saat scene start
    public void StartAmbient()
    {
        if (ambientClips == null) return;

        for (int i = 0; i < ambientClips.Length; i++)
        {
            if (ambientClips[i] == null) continue;

            GameObject ambientObj = new GameObject("AmbientLayer_" + ambientClips[i].name);
            AudioSource src = ambientObj.AddComponent<AudioSource>();
            src.clip = ambientClips[i];
            src.loop = true;
            src.spatialBlend = 0f;

            if (i == 0 && ambientMixerGroup != null)
                src.outputAudioMixerGroup = ambientMixerGroup;
            else if (i == 1 && clockMixerGroup != null)
                src.outputAudioMixerGroup = clockMixerGroup;

            src.volume = 1f; 
            src.Play();
        }
    }
    void Start()
    {
        // Load master volume
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        StartAmbient();
    }
}