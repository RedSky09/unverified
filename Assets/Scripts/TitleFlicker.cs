using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class TitleFlicker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text title;

    [Header("Timing")]
    [SerializeField] private float minInterval = 6f;
    [SerializeField] private float maxInterval = 12f;

    [Header("Effect")]
    [SerializeField] private int flickerCount = 3;
    [SerializeField] private float flickerSpeed = 0.04f;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color flickerColor = new Color(1f, 0.9f, 0.9f);

    private void Awake()
    {
        if (title == null)
            title = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            for (int i = 0; i < flickerCount; i++)
            {
                title.color = flickerColor;

                title.alpha = Random.Range(0.5f, 1f);

                yield return new WaitForSeconds(flickerSpeed);

                title.color = normalColor;
                title.alpha = 1f;

                yield return new WaitForSeconds(flickerSpeed * 0.6f);
            }
        }
    }
}