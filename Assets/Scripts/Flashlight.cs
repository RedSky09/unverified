using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    [Header("Light Component")]
    public Light spotLight;

    [Header("Battery")]
    public float maxBattery = 100f;
    public float drainRate = 2f;
    public float warningThreshold = 40f;
    public float criticalThreshold = 15f;

    [Header("Beam")]
    public float minBeamAngle = 20f;
    public float maxBeamAngle = 60f;

    [Header("Flicker")]
    public float flickerSpeed = 8f;

    [Header("UI")]
    public Image batteryFillImage;        

    [Header("Events")]
    public UnityEvent onWarning;
    public UnityEvent onCritical;
    public UnityEvent onDead;

    // State
    private float currentBattery;
    private bool isOn = false;
    private bool warningTriggered = false;
    private bool criticalTriggered = false;
    private bool deadTriggered = false;
    private float originalIntensity;

    // Key untuk PlayerPrefs
    private const string BATTERY_KEY = "FlashlightBattery";

    void Start()
    {
        // Load battery dari level sebelumnya
        // Kalau tidak ada data tersimpan, mulai dari max
        if (spotLight == null)
            spotLight = GetComponent<Light>();

        if (spotLight == null)
            Debug.LogError("Flashlight: Light component tidak ditemukan!");

        currentBattery = PlayerPrefs.GetFloat(BATTERY_KEY, maxBattery);

        // Auto-grab Light component kalau belum di-assign manual
        if (spotLight == null)
            spotLight = GetComponent<Light>();

        if (spotLight != null)
        {
            originalIntensity = spotLight.intensity;
            spotLight.enabled = false;
        }

        UpdateUI();
    }

    void Update()
    {
        if (!isOn || currentBattery <= 0) return;

        DrainBattery();
        HandleFlicker();
        UpdateUI();
        CheckThresholds();
    }

    void DrainBattery()
    {
        currentBattery -= drainRate * Time.deltaTime;
        currentBattery = Mathf.Max(currentBattery, 0f);

        // Simpan tiap frame — ini agak boros
        // Nanti bisa dioptimasi dengan simpan hanya saat level selesai
        PlayerPrefs.SetFloat(BATTERY_KEY, currentBattery);
    }

    void HandleFlicker()
    {
        if (spotLight == null) return;

        float batteryRatio = currentBattery / maxBattery;

        // Flicker hanya saat chunk 1 — battery <= 25%
        if (currentBattery <= 25f)
        {
            // Makin rendah battery, makin agresif flickernya
            float aggressiveness = 1f - (currentBattery / 25f); // 0 saat 25%, 1 saat 0%

            float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
            float minIntensity = Mathf.Lerp(originalIntensity * 0.4f, 0f, aggressiveness);
            float maxInt = originalIntensity * batteryRatio;

            spotLight.intensity = Mathf.Lerp(minIntensity, maxInt, flicker);
        }
        else
        {
            // Battery chunk 2-4 — cahaya normal, turun sesuai battery
            spotLight.intensity = originalIntensity * batteryRatio;
        }
    }
    void CheckThresholds()
    {
        if (!warningTriggered && currentBattery <= warningThreshold)
        {
            warningTriggered = true;
            onWarning.Invoke();

            // Trigger Mr. Voss battery warning
            VossDialog voss = FindFirstObjectByType<VossDialog>();
            if (voss != null) voss.ShowBatteryWarning();
        }

        if (!criticalTriggered && currentBattery <= criticalThreshold)
        {
            criticalTriggered = true;
            onCritical.Invoke();
        }

        if (!deadTriggered && currentBattery <= 0)
        {
            deadTriggered = true;
            onDead.Invoke();

            if (spotLight != null)
                spotLight.intensity = 0f;
        }
    }
    void UpdateUI()
    {
        if (batteryFillImage == null)
            return;

        float ratio = currentBattery / maxBattery;

        if (ratio <= 0f)
            batteryFillImage.fillAmount = 0f;
        else if (ratio <= 0.25f)
            batteryFillImage.fillAmount = 0.272f;
        else if (ratio <= 0.50f)
            batteryFillImage.fillAmount = 0.49f;
        else if (ratio <= 0.75f)
            batteryFillImage.fillAmount = 0.715f;
        else
            batteryFillImage.fillAmount = 1f;
    }

    // Toggle on/off — panggil dari input
    public void ToggleFlashlight()
    {
        if (currentBattery <= 0) return;
        isOn = !isOn;

        if (spotLight != null)
            spotLight.enabled = isOn;

        // Tambahkan ini
        if (AudioManager.Instance != null)
        {
            if (isOn) AudioManager.Instance.PlayFlashlightOn();
            else AudioManager.Instance.PlayFlashlightOff();
        }
    }

    // Beam angle — panggil dari input scroll atau key
    public void AdjustBeamAngle(float delta)
    {
        if (spotLight == null) return;

        spotLight.spotAngle = Mathf.Clamp(
            spotLight.spotAngle + delta,
            minBeamAngle,
            maxBeamAngle
        );
    }

    // Dipanggil saat player pickup battery item
    public void AddBattery(float amount)
    {
        currentBattery = Mathf.Min(currentBattery + amount, maxBattery);

        // Reset threshold kalau battery naik lagi
        if (currentBattery > warningThreshold) warningTriggered = false;
        if (currentBattery > criticalThreshold) criticalTriggered = false;
        deadTriggered = false;

        PlayerPrefs.SetFloat(BATTERY_KEY, currentBattery);
        UpdateUI();
    }

    // Reset penuh — dipanggil hanya di level 1 / game baru
    public void ResetBattery()
    {
        currentBattery = maxBattery;
        warningTriggered = false;
        criticalTriggered = false;
        deadTriggered = false;
        PlayerPrefs.SetFloat(BATTERY_KEY, currentBattery);
        UpdateUI();
    }

    public float GetBatteryRatio() => currentBattery / maxBattery;
}