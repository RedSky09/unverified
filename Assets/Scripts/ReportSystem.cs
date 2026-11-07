using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class ReportSystem : MonoBehaviour
{
    public enum AnomalyType
    {
        None,
        MissingObject,
        ExtraObject,
        PositionChanged,
        RotationChanged,
        ScaleChanged,
        Combined
    }

    [Header("UI Panels")]
    public GameObject reportPanel;
    public RectTransform panelNormal;
    public RectTransform panelAnomalyType;

    [Header("Phase 1 Buttons")]
    public Button btnNormal;
    public Button btnAnomaly;

    [Header("Phase 2 Buttons")]
    public Button btnMissing;
    public Button btnExtra;
    public Button btnPosition;
    public Button btnRotation;
    public Button btnScale;
    public Button btnBack;

    [Header("Animation")]
    public float slideDuration = 0.3f;
    public float panelSpacing = 320f;   

    [Header("Transition")]
    public float blackScreenDuration = 1.5f;
    public CanvasGroup blackScreen;

    [Header("State")]
    public bool isAnomalyLevel = false;
    public AnomalyType currentLevelAnomalyType = AnomalyType.None;

    [Header("Gameplay UI — hide saat transisi")]
    public GameObject levelText;
    public GameObject crosshair;
    public GameObject batteryUI;
    public GameObject interactPrompt;

    private bool panelOpen = false;
    private bool hasReported = false;
    private bool isAnimating = false;

    private Vector2 panel1DefaultPos;

    void Start()
    {
        reportPanel.SetActive(false);
        panelAnomalyType.gameObject.SetActive(false);

        // Simpan posisi dari editor, jangan override
        panel1DefaultPos = panelNormal.anchoredPosition;

        btnNormal.onClick.AddListener(() => SubmitNormal());
        btnAnomaly.onClick.AddListener(() => ShowAnomalyTypes());

        btnMissing.onClick.AddListener(() => SubmitAnomalyType(AnomalyType.MissingObject));
        btnExtra.onClick.AddListener(() => SubmitAnomalyType(AnomalyType.ExtraObject));
        btnPosition.onClick.AddListener(() => SubmitAnomalyType(AnomalyType.PositionChanged));
        btnRotation.onClick.AddListener(() => SubmitAnomalyType(AnomalyType.RotationChanged));
        btnScale.onClick.AddListener(() => SubmitAnomalyType(AnomalyType.ScaleChanged));
        btnBack.onClick.AddListener(() => BackToPhase1());
    }

    void Update()
    {
        if (hasReported) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
            TogglePanel();

        if (Keyboard.current.escapeKey.wasPressedThisFrame && panelOpen)
            ClosePanel();
    }

    void TogglePanel()
    {
        if (panelOpen) ClosePanel();
        else OpenPanel();
    }

    void OpenPanel()
    {
        panelOpen = true;
        reportPanel.SetActive(true);

        // Force posisi ke tengah
        panelNormal.anchoredPosition = panel1DefaultPos;
        panelNormal.gameObject.SetActive(true);
        panelAnomalyType.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        FirstPersonController fpc = FindFirstObjectByType<FirstPersonController>();
        if (fpc != null) fpc.enabled = false;
    }

    void ClosePanel()
    {
        panelOpen = false;
        reportPanel.SetActive(false);

        // Reset posisi untuk next open
        panelNormal.anchoredPosition = Vector2.zero;
        panelAnomalyType.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FirstPersonController fpc = FindFirstObjectByType<FirstPersonController>();
        if (fpc != null) fpc.enabled = true;
    }

    void ShowAnomalyTypes()
    {
        if (isAnimating) return;
        StartCoroutine(AnimateDualPanel());
    }

    IEnumerator AnimateDualPanel()
    {
        isAnimating = true;

        // Aktifkan panel 2 di luar layar kanan
        panelAnomalyType.gameObject.SetActive(true);
        panelAnomalyType.anchoredPosition = new Vector2(panelSpacing * 2f, 0f);

        Vector2 panel1Start = panelNormal.anchoredPosition;
        Vector2 panel1End = new Vector2(-panelSpacing, panel1DefaultPos.y);

        Vector2 panel2Start = new Vector2(panelSpacing * 2f, panel1DefaultPos.y);
        Vector2 panel2End = new Vector2(panelSpacing, panel1DefaultPos.y);

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);

            panelNormal.anchoredPosition = Vector2.Lerp(panel1Start, panel1End, t);
            panelAnomalyType.anchoredPosition = Vector2.Lerp(panel2Start, panel2End, t);

            yield return null;
        }

        panelNormal.anchoredPosition = panel1End;
        panelAnomalyType.anchoredPosition = panel2End;

        isAnimating = false;
    }

    void BackToPhase1()
    {
        if (isAnimating) return;
        StartCoroutine(AnimateBackToPhase1());
    }

    IEnumerator AnimateBackToPhase1()
    {
        isAnimating = true;

        Vector2 panel1Start = panelNormal.anchoredPosition;    
        Vector2 panel1End = panel1DefaultPos;                       

        Vector2 panel2Start = panelAnomalyType.anchoredPosition; 
        Vector2 panel2End = new Vector2(panelSpacing * 2f, 0f);  

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);

            panelNormal.anchoredPosition = Vector2.Lerp(panel1Start, panel1End, t);
            panelAnomalyType.anchoredPosition = Vector2.Lerp(panel2Start, panel2End, t);

            yield return null;
        }

        panelNormal.anchoredPosition = panel1End;
        panelAnomalyType.gameObject.SetActive(false);

        isAnimating = false;
    }

    void SubmitNormal()
    {
        hasReported = true;
        ClosePanel();
        bool isCorrect = !isAnomalyLevel;
        TriggerJumpscareAndTransition(isCorrect);
    }

    void SubmitAnomalyType(AnomalyType reportedType)
    {
        hasReported = true;
        ClosePanel();

        bool isCorrect = isAnomalyLevel &&
                         (reportedType == currentLevelAnomalyType);

        TriggerJumpscareAndTransition(isCorrect);
    }

    void TriggerJumpscareAndTransition(bool isCorrect)
    {
        SetGameplayUIVisible(false);
        StartCoroutine(TransitionRoutine(isCorrect));
    }

    void SetGameplayUIVisible(bool visible)
    {
        if (levelText != null) levelText.SetActive(visible);
        if (crosshair != null) crosshair.SetActive(visible);
        if (batteryUI != null) batteryUI.SetActive(visible);
        if (interactPrompt != null) interactPrompt.SetActive(visible);
    }

    IEnumerator TransitionRoutine(bool isCorrect)
    {
        yield return StartCoroutine(FadeBlackScreen(0f, 1f));
        yield return new WaitForSeconds(blackScreenDuration);

        if (isCorrect)
        {
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

            if (currentLevel >= GameManager.MAX_LEVELS)
            {
                GameManager.OnCorrectAnswer();
                yield break;
            }

            GameManager.OnCorrectAnswer();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            GameManager.OnWrongAnswer();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator FadeBlackScreen(float from, float to)
    {
        if (blackScreen == null) yield break;

        float elapsed = 0f;
        float duration = 0.8f;

        while (elapsed < duration)
        {
            if (blackScreen == null) yield break;
            elapsed += Time.deltaTime;
            blackScreen.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        blackScreen.alpha = to;
    }
}