using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public static bool IsFirstPlay { get; private set; } = false;

    [Header("Level State")]
    public bool isAnomalyLevel = false;

    [Header("References")]
    public ReportSystem reportSystem;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        bool isFirstPlay = PlayerPrefs.GetInt("HasPlayed", 0) == 0;

        if (currentLevel == 1 && isFirstPlay)
        {
            isAnomalyLevel = false;
            IsFirstPlay = true;  // set static field 
            PlayerPrefs.SetInt("HasPlayed", 1);
            PlayerPrefs.Save();
        }
        else
        {
            IsFirstPlay = false;
            isAnomalyLevel = Random.value < 0.6f;
        }

        if (reportSystem != null)
            reportSystem.isAnomalyLevel = isAnomalyLevel;

        SetAnomalyState(isAnomalyLevel);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StartAmbient();
    }

    void SetAnomalyState(bool anomaly)
    {
        AnomalyGroup[] groups = FindObjectsByType<AnomalyGroup>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        AnomalyObject[] allObjects = FindObjectsByType<AnomalyObject>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        // Filter standalone
        System.Collections.Generic.List<AnomalyObject> standaloneObjects =
            new System.Collections.Generic.List<AnomalyObject>();

        foreach (AnomalyObject obj in allObjects)
        {
            bool isInGroup = false;
            foreach (AnomalyGroup group in groups)
                foreach (AnomalyObject member in group.groupMembers)
                    if (member == obj) { isInGroup = true; break; }
            if (!isInGroup) standaloneObjects.Add(obj);
        }

        // Reset semua
        foreach (AnomalyGroup g in groups) g.SetAnomaly(false);
        foreach (AnomalyObject o in standaloneObjects) o.SetAnomaly(false);

        if (!anomaly)
        {
            if (reportSystem != null)
                reportSystem.currentLevelAnomalyType = ReportSystem.AnomalyType.None;
            return;
        }

        // Build pool
        System.Collections.Generic.List<object> pool =
            new System.Collections.Generic.List<object>();
        foreach (AnomalyGroup g in groups) pool.Add(g);
        foreach (AnomalyObject o in standaloneObjects) pool.Add(o);

        // Shuffle
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int anomalyCount = GetAnomalyCount(currentLevel);
        anomalyCount = Mathf.Min(anomalyCount, pool.Count);

        ReportSystem.AnomalyType activeType = ReportSystem.AnomalyType.None;

        for (int i = 0; i < anomalyCount; i++)
        {
            if (pool[i] is AnomalyGroup group)
            {
                group.SetAnomaly(true);
                if (group.groupMembers.Length > 0 && i == 0)
                    activeType = group.groupMembers[0].GetAnomalyType();
            }
            else if (pool[i] is AnomalyObject obj)
            {
                obj.SetAnomaly(true);
                if (i == 0) activeType = obj.GetAnomalyType();
            }
        }
        if (reportSystem != null)
            reportSystem.currentLevelAnomalyType = activeType;
    }
    int GetAnomalyCount(int level)
    {
        return 1;
    }
}