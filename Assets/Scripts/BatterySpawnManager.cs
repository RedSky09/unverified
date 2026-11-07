using UnityEngine;
using System.Collections.Generic;

public class BatterySpawnManager : MonoBehaviour
{
    [Header("Battery Spots")]
    public GameObject[] batterySpots;    // Semua titik yang mungkin ada battery

    [Header("Settings")]
    public int maxBatteriesPerLevel = 2;
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;     // Chance tiap spot punya battery

    void Start()
    {
        SpawnBatteries();
    }

    void SpawnBatteries()
    {
        if (batterySpots == null || batterySpots.Length == 0) return;

        // Nonaktifkan semua spot dulu
        foreach (GameObject spot in batterySpots)
        {
            if (spot != null)
                spot.SetActive(false);
        }

        // Shuffle spots supaya random
        List<GameObject> shuffled = new List<GameObject>(batterySpots);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        // Aktifkan maksimal sesuai maxBatteriesPerLevel
        int spawned = 0;
        foreach (GameObject spot in shuffled)
        {
            if (spawned >= maxBatteriesPerLevel) break;

            // Chance check
            if (Random.value <= spawnChance)
            {
                spot.SetActive(true);
                spawned++;
            }
        }
    }
}