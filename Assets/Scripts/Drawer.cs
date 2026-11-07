using UnityEngine;
using System.Collections;

public class Drawer : MonoBehaviour, IInteractable
{
    [Header("Animation")]
    public Vector3 openOffset = new Vector3(0f, 0f, 0.18f);
    public float openDuration = 0.4f;

    [Header("Audio")]
    public AudioSource drawerAudio;
    public AudioClip openSound;

    private Vector3 closedPosition;
    private bool isAnimating = false;
    private bool hasBeenOpened = false;

    void Start()
    {
        closedPosition = transform.localPosition;
    }

    public void Interact()
    {
        if (isAnimating || hasBeenOpened) return;
        hasBeenOpened = true;
        StartCoroutine(OpenDrawer());
    }

    IEnumerator OpenDrawer()
    {
        isAnimating = true;

        // Hapus kode audio lama, ganti dengan:
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDrawerSlide();

        float elapsed = 0f;
        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = closedPosition + openOffset;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / openDuration);
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.localPosition = targetPos;
        isAnimating = false;
    }
}