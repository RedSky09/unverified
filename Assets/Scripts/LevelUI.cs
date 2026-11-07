using UnityEngine;
using TMPro;

public class LevelUI : MonoBehaviour
{
    private TextMeshProUGUI levelText;

    void Start()
    {
        levelText = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    void UpdateText()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        levelText.text = "ROOM " + currentLevel + " / " + GameManager.MAX_LEVELS;
    }
}