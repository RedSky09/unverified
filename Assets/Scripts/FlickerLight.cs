using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class HDRPFlickerLight : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Light targetLight;

    [Header("Base — auto set dari light")]
    [HideInInspector] public float normalIntensity;

    [Header("Flicker Depth")]
    [Range(0f, 1f)]
    public float flickerDepth = 0.5f;      // 0 = tidak flicker, 1 = mati total
    [Range(0f, 1f)]
    public float ambientVariation = 0.08f; // Variasi ambient halus

    [Header("Speed")]
    public float ambientSpeed = 0.2f;
    public float smoothSpeed = 15f;

    [Header("Timing")]
    public float minInterval = 6f;
    public float maxInterval = 20f;

    [Header("Rare Heavy Flicker")]
    [Range(0f, 1f)]
    public float heavyChance = 0.2f;

    [Header("Color Shift")]
    public bool enableColorShift = true;
    public Color warmColor = new Color(1f, 0.86f, 0.58f);
    public Color flickerColor = new Color(1f, 0.75f, 0.4f);

    private float noiseSeed;
    private float currentIntensity;
    private float targetIntensity;
    private bool flickering;

    void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        noiseSeed = Random.Range(0f, 999f);

        // Auto-grab intensity dan color dari light
        normalIntensity = targetLight.intensity;
        warmColor = targetLight.color;
        flickerColor = new Color(
            warmColor.r,
            warmColor.g * 0.85f,
            warmColor.b * 0.6f
        );

        currentIntensity = normalIntensity;
        targetIntensity = normalIntensity;
    }

    void Start()
    {
        StartCoroutine(FlickerLoop());
    }

    void Update()
    {
        if (!flickering)
        {
            // Ambient breathing — variasi halus terus menerus
            float noise = Mathf.PerlinNoise(noiseSeed, Time.time * ambientSpeed);
            noise = (noise - 0.5f) * 2f;
            targetIntensity = normalIntensity + noise * (normalIntensity * ambientVariation);
        }

        // Smooth interpolation
        currentIntensity = Mathf.Lerp(
            currentIntensity,
            targetIntensity,
            Time.deltaTime * smoothSpeed
        );

        targetLight.intensity = Mathf.Max(0f, currentIntensity);

        // Color shift halus saat flicker
        if (enableColorShift)
        {
            float t = 1f - (currentIntensity / normalIntensity);
            targetLight.color = Color.Lerp(warmColor, flickerColor, t * 0.5f);
        }
    }

    IEnumerator FlickerLoop()
    {
        while (true)
        {
            // Tunggu interval random
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            flickering = true;

            if (Random.value < heavyChance)
                yield return HeavyPattern();
            else
                yield return NormalPattern();

            // Smooth kembali ke normal
            float elapsed = 0f;
            float returnDuration = 0.1f;
            float startIntensity = currentIntensity;

            while (elapsed < returnDuration)
            {
                elapsed += Time.deltaTime;
                targetIntensity = Mathf.Lerp(startIntensity, normalIntensity, elapsed / returnDuration);
                yield return null;
            }

            targetIntensity = normalIntensity;
            flickering = false;
        }
    }

    IEnumerator NormalPattern()
    {
        // Flicker 2-3 kali cepat
        int flickerCount = Random.Range(2, 4);
        for (int i = 0; i < flickerCount; i++)
        {
            // Drop
            targetIntensity = normalIntensity * (1f - flickerDepth * Random.Range(0.3f, 0.7f));
            yield return new WaitForSeconds(Random.Range(0.03f, 0.07f));

            // Recover
            targetIntensity = normalIntensity * Random.Range(0.9f, 1.1f);
            yield return new WaitForSeconds(Random.Range(0.02f, 0.05f));
        }
    }

    IEnumerator HeavyPattern()
    {
        // Drop dramatis
        targetIntensity = normalIntensity * (1f - flickerDepth);
        yield return new WaitForSeconds(0.05f);

        // Mati sebentar
        targetIntensity = normalIntensity * 0.05f;
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));

        // Flicker balik
        for (int i = 0; i < 3; i++)
        {
            targetIntensity = normalIntensity * Random.Range(0.4f, 0.8f);
            yield return new WaitForSeconds(0.04f);
            targetIntensity = normalIntensity * 0.1f;
            yield return new WaitForSeconds(0.03f);
        }

        // Kembali normal
        targetIntensity = normalIntensity;
    }
}