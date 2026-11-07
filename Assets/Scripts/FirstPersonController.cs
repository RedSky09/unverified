using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Look")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Spawn")]
    public Transform spawnPoint;

    [Header("Flashlight")]
    public Flashlight flashlight;

    [Header("Headbob")]
    public float bobFrequency = 2f;
    public float bobAmplitude = 0.05f;
    public float bobSmoothSpeed = 10f;

    private float bobTimer = 0f;
    private Vector3 originalCameraPos;

    private CharacterController characterController;
    private Camera playerCamera;
    private float verticalRotation = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // Load sensitivity dari PlayerPrefs
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2f);

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;  // tambahkan ini

            // Reset vertical rotation kamera juga
            verticalRotation = 0f;
            playerCamera.transform.localRotation = Quaternion.identity;
        }

        originalCameraPos = playerCamera.transform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleHeadbob();

        // Toggle flashlight — F key
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (flashlight != null)
                flashlight.ToggleFlashlight();
        }

        // Beam angle — scroll wheel
        if (Mouse.current.scroll.ReadValue().y != 0)
        {
            if (flashlight != null)
                flashlight.AdjustBeamAngle(Mouse.current.scroll.ReadValue().y * 0.1f);
        }
    }

    void HandleMovement()
    {
        Vector2 moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // Gravity sederhana
        characterController.Move(Vector3.down * 9.8f * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * 0.05f;
        float mouseY = mouseDelta.y * mouseSensitivity * 0.05f;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleHeadbob()
    {
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

        bool isMoving = moveInput.magnitude > 0;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            Vector3 bobOffset = new Vector3(
                Mathf.Sin(bobTimer * 0.5f) * bobAmplitude,
                Mathf.Sin(bobTimer) * bobAmplitude,
                0f
            );
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                originalCameraPos + bobOffset,
                Time.deltaTime * bobSmoothSpeed
            );
        }
        else
        {
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                originalCameraPos,
                Time.deltaTime * bobSmoothSpeed
            );
            bobTimer = 0f;
        }
    }
}