using UnityEngine;

public class BatteryPickup : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public float refillAmount = 40f; 

    [Header("Audio")]
    public AudioSource pickupAudio;
    public AudioClip pickupSound;

    private bool collected = false;

    public void Interact()
    {
        if (collected) return;
        collected = true;

        // Cari flashlight dan refill battery
        Flashlight flashlight = FindFirstObjectByType<Flashlight>();
        if (flashlight != null)
        {
            flashlight.AddBattery(refillAmount);
            Debug.Log("Battery +30%");
        }

        // Hapus kode audio lama, ganti dengan:
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBatteryPickup();

        // Sembunyikan object
        gameObject.SetActive(false);
    }
}