using System;
using System.Collections;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeOutDuration = 0.4f;
    [SerializeField] private float fadeInDuration = 0.5f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void PlayFadeOut(Action onComplete)
    {
        StartCoroutine(FadeOutRoutine(onComplete));
    }

    private IEnumerator FadeOutRoutine(Action onComplete)
    {
        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = 0f;
        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeOutDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
        onComplete?.Invoke();
    }

    public void PlayFadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        fadeCanvasGroup.alpha = 1f;
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeInDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false);
    }
}