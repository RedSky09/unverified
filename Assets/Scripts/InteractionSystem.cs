using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionSystem : MonoBehaviour
{
    [Header("Raycast")]
    public float interactDistance = 2.5f;
    public LayerMask interactLayer;

    [Header("UI")]
    public Image crosshair;
    public TMPro.TextMeshProUGUI interactPrompt;
    public Color normalColor = new Color(1f, 1f, 1f, 0.7f);
    public Color hoverColor = new Color(1f, 1f, 0f, 1f);   

    private Camera playerCamera;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        CheckInteractable();
        HandleInput();
    }

    void CheckInteractable()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (crosshair != null)
                    crosshair.color = hoverColor;

                // Tampilkan prompt sesuai tipe object
                if (interactPrompt != null)
                {
                    if (hit.collider.GetComponent<Drawer>() != null)
                        interactPrompt.text = "[E]  Open";
                    else if (hit.collider.GetComponent<BatteryPickup>() != null)
                        interactPrompt.text = "[E]  Pick Up Battery";
                    else
                        interactPrompt.text = "[E]  Interact";
                }
                return;
            }
        }

        // Tidak ada interactable
        if (crosshair != null)
            crosshair.color = normalColor;

        if (interactPrompt != null)
            interactPrompt.text = "";
    }

    void HandleInput()
    {
        if (!UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Debug.Log("E ditekan");

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
                interactable.Interact();
            else
                Debug.Log("Tidak ada IInteractable di object ini");
        }
        else
        {
            Debug.Log("Raycast tidak kena apa-apa");
        }
    }
}

// Interface untuk semua object yang bisa di-interact
public interface IInteractable
{
    void Interact();
}