using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class InGameFlickerLight : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Light targetLight;

    [Header("Base — auto set dari light")]
    [HideInInspector] public float normalIntensity;
    [HideInInspector] public float normalRange;

    [Header("Flicker Depth")]
    [Range(0f, 1f)]
    public float flickerDepth = 0.5f;
    [Range(0f, 1f)]
    public float ambientVariation = 0.08f;

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
    private Color warmColor;
    private Color flickerColor;

    [Header("Range Variation (optional)")]
    public bool enableRangeFlicker = false;
    [Range(0.3f, 1f)]
    public float minRangeFlicker = 0.7f;

    private float noiseSeed;
    private float currentIntensity;
    private float targetIntensity;
    private bool flickering;

    void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        noiseSeed = Random.Range(0f, 999f);

        normalIntensity = targetLight.intensity;
        normalRange = targetLight.range;

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
            float noise = Mathf.PerlinNoise(noiseSeed, Time.time * ambientSpeed);
            noise = (noise - 0.5f) * 2f;
            targetIntensity = normalIntensity + noise * (normalIntensity * ambientVariation);
        }

        currentIntensity = Mathf.Lerp(
            currentIntensity,
            targetIntensity,
            Time.deltaTime * smoothSpeed
        );

        targetLight.intensity = Mathf.Max(0f, currentIntensity);

        if (enableColorShift)
        {
            float t = 1f - (currentIntensity / normalIntensity);
            targetLight.color = Color.Lerp(warmColor, flickerColor, Mathf.Clamp01(t * 0.5f));
        }

        if (enableRangeFlicker)
        {
            float intensityRatio = Mathf.Clamp01(currentIntensity / normalIntensity);
            targetLight.range = Mathf.Lerp(
                normalRange * minRangeFlicker,
                normalRange,
                intensityRatio
            );
        }
    }

    IEnumerator FlickerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            flickering = true;

            if (Random.value < heavyChance)
                yield return HeavyPattern();
            else
                yield return NormalPattern();

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
        int flickerCount = Random.Range(2, 4);
        for (int i = 0; i < flickerCount; i++)
        {
            targetIntensity = normalIntensity * (1f - flickerDepth * Random.Range(0.3f, 0.7f));
            yield return new WaitForSeconds(Random.Range(0.03f, 0.07f));

            targetIntensity = normalIntensity * Random.Range(0.9f, 1.1f);
            yield return new WaitForSeconds(Random.Range(0.02f, 0.05f));
        }
    }

    IEnumerator HeavyPattern()
    {
        targetIntensity = normalIntensity * (1f - flickerDepth);
        yield return new WaitForSeconds(0.05f);

        targetIntensity = normalIntensity * 0.05f;
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));

        for (int i = 0; i < 3; i++)
        {
            targetIntensity = normalIntensity * Random.Range(0.4f, 0.8f);
            yield return new WaitForSeconds(0.04f);
            targetIntensity = normalIntensity * 0.1f;
            yield return new WaitForSeconds(0.03f);
        }

        targetIntensity = normalIntensity;
    }
}