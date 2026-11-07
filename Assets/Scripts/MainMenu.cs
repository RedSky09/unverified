using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject buttonGroup;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    [Header("Transition")]
    public CanvasGroup canvasGroup;
    public float fadeInDuration = 1.5f;

    [Header("Settings Panel")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityValueText;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;
    public Toggle fullscreenToggle;

    [Header("Main Elements")]
    public GameObject titleGroup;

    void Start()
    {
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
            UpdateSensitivityText(sensitivitySlider.value);
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
            UpdateVolumeText(volumeSlider.value);
            AudioListener.volume = volumeSlider.value;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (fullscreenToggle != null)
        {
            bool isFullscreen = PlayerPrefs.GetInt("IsFullscreen", 1) == 1;
            fullscreenToggle.isOn = isFullscreen;
            ApplyFullscreen(isFullscreen);
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.ResetGame();

        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        StartCoroutine(FadeIn());
    }

    public void OnFullscreenToggle(bool isFullscreen)
    {
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
        ApplyFullscreen(isFullscreen);
    }

    void ApplyFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
    }

    IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public void OnPlay()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        if (canvasGroup != null)
        {
            float elapsed = 0f;
            float duration = 0.8f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }
        }

        SceneManager.LoadScene("Demo_Scene");
    }

    public void OnSettings()
    {
        if (settingsPanel == null) return;
        bool isOpen = settingsPanel.activeSelf;

        settingsPanel.SetActive(!isOpen);

        if (buttonGroup != null) buttonGroup.SetActive(isOpen);
        if (titleGroup != null) titleGroup.SetActive(isOpen);
    }

    public void OnCredits()
    {
        if (creditsPanel == null) return;
        bool isOpen = creditsPanel.activeSelf;

        creditsPanel.SetActive(!isOpen);

        if (buttonGroup != null) buttonGroup.SetActive(isOpen);
        if (titleGroup != null) titleGroup.SetActive(isOpen);
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        UpdateSensitivityText(value);
    }

    public void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        AudioListener.volume = value;
        UpdateVolumeText(value);
    }

    void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
            sensitivityValueText.text = value.ToString("F1");
    }

    void UpdateVolumeText(float value)
    {
        if (volumeValueText != null)
            volumeValueText.text = Mathf.RoundToInt(value * 100) + "%";
    }
}