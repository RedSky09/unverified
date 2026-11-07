using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public const int MAX_LEVELS = 9;

    void Awake()
    {
        Instance = this;
    }

    // Dipanggil saat player benar
    public static void OnCorrectAnswer()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int mistakes = PlayerPrefs.GetInt("TotalMistakes", 0);

        if (currentLevel >= MAX_LEVELS)
        {
            // Langsung trigger ending tanpa increment
            TriggerEnding(mistakes);
            return;
        }

        // Increment level
        PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);
        PlayerPrefs.Save();
    }

    // Dipanggil saat player salah
    public static void OnWrongAnswer()
    {
        int mistakes = PlayerPrefs.GetInt("TotalMistakes", 0);
        mistakes++;
        PlayerPrefs.SetInt("TotalMistakes", mistakes);

        // Reset ke level 1
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
    }

    static void TriggerEnding(int mistakes)
    {

        if (mistakes <= 1)
            SceneManager.LoadScene("EndingTrue");
        else if (mistakes <= 3)
            SceneManager.LoadScene("EndingNeutral");
        else
            SceneManager.LoadScene("EndingBad");
    }

    // Reset semua data — dipanggil saat new game
    public static void ResetGame()
    {
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("TotalMistakes", 0);
        PlayerPrefs.SetInt("HasPlayed", 0);  // tambahkan ini
        PlayerPrefs.SetFloat("FlashlightBattery", 100f);
        PlayerPrefs.Save();
    }
}