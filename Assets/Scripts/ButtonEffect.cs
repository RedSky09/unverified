using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Text")]
    [SerializeField] private TMP_Text text;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0.82f, 0.78f, 0.72f);
    [SerializeField] private Color hoverColor = Color.white;

    [Header("Animation")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("Glow")]
    [SerializeField] private bool useGlow = true;
    [SerializeField] private float glowPower = 0.4f;

    private Vector3 targetScale;
    private Material materialInstance;

    private void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();

        targetScale = Vector3.one;

        text.color = normalColor;

        // Buat material instance supaya glow tiap button independen
        materialInstance = Instantiate(text.fontSharedMaterial);
        text.fontSharedMaterial = materialInstance;

        if (useGlow)
            materialInstance.SetFloat(ShaderUtilities.ID_GlowPower, 0);
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
        targetScale = Vector3.one * hoverScale;

        if (useGlow)
            materialInstance.SetFloat(ShaderUtilities.ID_GlowPower, glowPower);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetVisual();
    }

    private void OnDisable()
    {
        ResetVisual();
    }

    private void ResetVisual()
    {
        text.color = normalColor;
        targetScale = Vector3.one;

        if (useGlow && materialInstance != null)
            materialInstance.SetFloat(ShaderUtilities.ID_GlowPower, 0);
    }
}