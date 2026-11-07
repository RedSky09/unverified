using UnityEngine;

public class MenuCameraIdle : MonoBehaviour
{
    [Header("Position")]
    public Vector3 positionAmplitude = new Vector3(0.025f, 0.015f, 0.02f);

    [Header("Rotation")]
    public Vector3 rotationAmplitude = new Vector3(0.4f, 0.6f, 0.15f);

    [Header("Speed")]
    public float movementSpeed = 0.12f;

    [Header("Breathing")]
    public float breathingSpeed = 0.22f;
    public float breathingAmplitude = 0.005f;

    [Header("Delay")]
    public float startDelay = 3f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private float seedX;
    private float seedY;
    private float seedZ;

    private void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(100f, 200f);
        seedZ = Random.Range(200f, 300f);
    }

    private void Update()
    {
        if (Time.time < startDelay)
            return;

        float t = (Time.time - startDelay);

        float x = (Mathf.PerlinNoise(seedX, t * movementSpeed * 0.8f) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(seedY, t * movementSpeed * 1.3f) - 0.5f) * 2f;
        float z = (Mathf.PerlinNoise(seedZ, t * movementSpeed * 0.6f) - 0.5f) * 2f;

        float breathing = Mathf.Sin(t * breathingSpeed) * breathingAmplitude;

        Vector3 offset = new Vector3(
            x * positionAmplitude.x,
            y * positionAmplitude.y + breathing,
            z * positionAmplitude.z
        );

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            startPosition + offset,
            Time.deltaTime * 2f);

        Quaternion targetRotation =
            startRotation *
            Quaternion.Euler(
                y * rotationAmplitude.x,
                x * rotationAmplitude.y,
                z * rotationAmplitude.z);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * 2f);
    }
}