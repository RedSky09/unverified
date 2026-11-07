using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EndingScreen : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subText;
    public Button playAgainButton;
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    public string endingType = "true";

    [Header("Timing")]
    public float titleFadeInDuration = 1.5f;
    public float subtextDelay = 1f;
    public float subtextFadeInDuration = 1f;
    public float buttonDelay = 1.5f;
    public float buttonFadeInDuration = 0.8f;

    private CanvasGroup titleGroup;
    private CanvasGroup subGroup;
    private CanvasGroup buttonGroup;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Camera cam = Camera.main;
        if (cam != null)
            cam.backgroundColor = Color.black;

        titleGroup = GetOrAddCanvasGroup(titleText.gameObject);
        subGroup = GetOrAddCanvasGroup(subText.gameObject);
        buttonGroup = GetOrAddCanvasGroup(playAgainButton.gameObject);

        titleGroup.alpha = 0f;
        subGroup.alpha = 0f;
        buttonGroup.alpha = 0f;

        // Paksa interactable dari awal — jangan tunggu coroutine
        playAgainButton.interactable = true;

        SetEndingContent();
        StartCoroutine(PlayEndingSequence());
    }

    CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();
        return cg;
    }

    void SetEndingContent()
    {
        switch (endingType)
        {
            case "true":
                titleText.text = "THE TRUTH";
                subText.text = "You saw what others refused to see.\nMr. Voss has been informed.\n\nSome jobs aren't worth the truth they uncover.";
                break;

            case "neutral":
                titleText.text = "CASE CLOSED";
                subText.text = "Your report has been filed.\nPayment received. No further questions.\n\nIgnorance, it turns out, pays rather well.";
                break;

            case "bad":
                titleText.text = "UNRELIABLE";
                subText.text = "Your services are no longer required.\nSomeone else will finish the job.\n\nPerhaps inspection work isn't for everyone.";
                break;
        }
    }

    IEnumerator PlayEndingSequence()
    {
        // Tunggu sebentar sebelum mulai
        yield return new WaitForSeconds(0.5f);

        // Title fade in
        yield return StartCoroutine(FadeElement(titleGroup, 0f, 1f, titleFadeInDuration));

        // Subtext delay lalu fade in
        yield return new WaitForSeconds(subtextDelay);
        yield return StartCoroutine(FadeElement(subGroup, 0f, 1f, subtextFadeInDuration));

        // Tombol delay lalu fade in
        yield return new WaitForSeconds(buttonDelay);
        yield return StartCoroutine(FadeElement(buttonGroup, 0f, 1f, buttonFadeInDuration));

        // Aktifkan tombol setelah muncul
        playAgainButton.interactable = true;
    }

    IEnumerator FadeElement(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        cg.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        cg.alpha = to;
    }

    public void RestartGame()
    {
        GameManager.ResetGame();
        SceneManager.LoadScene(0); 
    }
}