using UnityEngine;

public class EndingAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverClip;
    public AudioClip clickClip;

    public void OnButtonHover()
    {
        if (audioSource != null && hoverClip != null)
            audioSource.PlayOneShot(hoverClip, 0.4f);
    }

    public void OnButtonClick()
    {
        if (audioSource != null && clickClip != null)
            audioSource.PlayOneShot(clickClip, 0.6f);
    }
}