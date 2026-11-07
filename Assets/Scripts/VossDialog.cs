using UnityEngine;
using TMPro;
using System.Collections;

public class VossDialog : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup dialogCanvasGroup;
    public TextMeshProUGUI dialogText;

    [Header("Audio")]
    public AudioSource radioAudio;
    public AudioClip radioBeep;

    [Header("Timing")]
    public float fadeInDuration = 0.3f;
    public float displayDuration = 3f;
    public float fadeOutDuration = 0.8f;
    public float typewriterSpeed = 0.04f;

    [Header("Briefing Lines — Accurate")]
    [TextArea]
    public string[] briefingLines = {
        "This room was last inspected three days ago. Standard check.",
        "The client requested a thorough inspection. Miss nothing.",
        "Unusual activity was reported here. Verify the room's condition.",
        "Routine inspection. The client provided no further details.",
        "This room has been vacant for some time. Check carefully.",
        "The previous report was incomplete. You will determine the condition.",
        "My client does not like to wait. Do your job.",
        "Something is different here. I can feel it.",              
        "This is the final inspection. Make sure your report is accurate."  
    };

    [Header("Briefing Lines — Misleading (aktif mulai level 4)")]
    [TextArea]
    public string[] misleadingBriefingLines = {
        "Everything here is exactly as it should be. No concerns.",
        "The previous inspector confirmed this room. Nothing unusual.",
        "My client assures me this room is untouched. Verify that.",
        "Standard condition. The client expects a clean report.",
        "Nothing has changed here. I can guarantee that.",
    };

    [Header("Hint Lines")]
    [TextArea]
    public string[] hintLines = {
        "Check every corner.",
        "Trust your eyes, not your assumptions.",
        "Something feels off. Or does it?",
        "Battery won't last forever.",
        "Details matter.",
        "Try your best.",
        "Keep going.",
        "Not everything is what it seems.",
        "The room doesn't lie. People do.",
        "Focus.",
        "Look carefully before you decide.",
        "You are being watched. Or are you?",
        "I wouldn't lie to you. Would I?",
        "Trust the briefing. Or don't.",
    };

    [Header("Battery Warning Lines")]
    [TextArea]
    public string[] batteryWarningLines = {
        "Your light is fading. Make your decision.",
        "I suggest you wrap this up.",
        "Time is running out. So is your battery.",
        "Darkness suits this room, don't you think?",
    };

    [Header("Misleading Chance")]

    [Header("Introduction — Level 1 First Play Only")]
    [TextArea]
    public string introLine1 =
    "My name is Voss. I arrange inspections for clients who prefer discretion.";
    [TextArea]
    public string introLine2 =
        "You are here because you were recommended. Do not make me regret that.";
    [TextArea]
    public string introLine3 =
        "This is a standard room. Normal conditions. Every corner, every detail matters.";

    [Range(0f, 1f)]
    public float misleadingChance = 0.4f; 

    private bool batteryWarningShown = false;

    void Start()
    {
        if (radioAudio != null && radioBeep == null)
            radioBeep = BeepGenerator.GenerateBeep();

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ShowOpeningDialog());
    }

    IEnumerator ShowOpeningDialog()
    {
        yield return new WaitForSeconds(1f);

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        bool isFirstPlay = LevelManager.IsFirstPlay;  // baca dari LevelManager 

        if (currentLevel == 1 && isFirstPlay)
        {
            yield return StartCoroutine(ShowLine(introLine1));
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(ShowLine(introLine2));
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(ShowLine(introLine3));
        }
        else
        {
            string briefing = GetBriefingLine(currentLevel);
            yield return StartCoroutine(ShowLine(briefing));

            yield return new WaitForSeconds(0.8f);

            string hint = hintLines[Random.Range(0, hintLines.Length)];
            yield return StartCoroutine(ShowLine(hint));
        }
    }
    string GetBriefingLine(int level)
    {
        // Level 1-2 selalu akurat
        if (level <= 2)
        {
            int index = Mathf.Clamp(level - 1, 0, briefingLines.Length - 1);
            return briefingLines[index];
        }

        // Level 3+ — ada chance misleading
        bool isMisleading = Random.value <= misleadingChance;

        if (isMisleading && misleadingBriefingLines.Length > 0)
        {
            return misleadingBriefingLines[Random.Range(0, misleadingBriefingLines.Length)];
        }
        else
        {
            int index = Mathf.Clamp(level - 1, 0, briefingLines.Length - 1);
            return briefingLines[index];
        }
    }

    // Dipanggil dari Flashlight.cs saat battery masuk chunk 1
    public void ShowBatteryWarning()
    {
        if (batteryWarningShown) return;
        batteryWarningShown = true;

        string warning = batteryWarningLines[Random.Range(0, batteryWarningLines.Length)];
        StartCoroutine(ShowLine(warning));
    }

    IEnumerator ShowLine(string line)
    {
        if (radioAudio != null && radioBeep != null)
            radioAudio.PlayOneShot(radioBeep);

        dialogText.text = "";
        dialogCanvasGroup.alpha = 0f;

        yield return StartCoroutine(FadeCanvas(0f, 1f, fadeInDuration));
        yield return StartCoroutine(TypewriterEffect(line));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeCanvas(1f, 0f, fadeOutDuration));

        dialogText.text = "";
    }

    IEnumerator TypewriterEffect(string line)
    {
        dialogText.text = "";
        foreach (char c in line)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float elapsed = 0f;
        dialogCanvasGroup.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            dialogCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        dialogCanvasGroup.alpha = to;
    }

    public void ShowCustomLine(string line)
    {
        StartCoroutine(ShowLine(line));
    }
}