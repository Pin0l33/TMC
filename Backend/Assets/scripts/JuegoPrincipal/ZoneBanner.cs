using System.Collections;
using UnityEngine;
using TMPro;

public class ZoneBanner : MonoBehaviour
{
    public static ZoneBanner Instance;

    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text zoneName;

    [Header("Animación")]
    [SerializeField] private float fadeInDuration = 0.35f;
    [SerializeField] private float visibleDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float movimiento = 40f;

    private RectTransform rectTransform;
    private Vector2 posicionFinal;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        Instance = this;

        rectTransform = GetComponent<RectTransform>();
        posicionFinal = rectTransform.anchoredPosition;

        canvasGroup.alpha = 0f;
    }

    public void MostrarZona(string nombre)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(MostrarZonaCoroutine(nombre));
    }

    private IEnumerator MostrarZonaCoroutine(string nombre)
    {
        zoneName.text = nombre;

        Vector2 posicionInicial = posicionFinal + Vector2.up * movimiento;

        rectTransform.anchoredPosition = posicionInicial;
        canvasGroup.alpha = 0f;

        float tiempo = 0f;

        while (tiempo < fadeInDuration)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Clamp01(tiempo / fadeInDuration);
            float suavizado = 1f - Mathf.Pow(1f - progreso, 3f);

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, suavizado);
            rectTransform.anchoredPosition = Vector2.Lerp(
                posicionInicial,
                posicionFinal,
                suavizado
            );

            yield return null;
        }

        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = posicionFinal;

        yield return new WaitForSeconds(visibleDuration);

        tiempo = 0f;

        while (tiempo < fadeOutDuration)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Clamp01(tiempo / fadeOutDuration);
            float suavizado = progreso * progreso * progreso;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, suavizado);
            rectTransform.anchoredPosition = Vector2.Lerp(
                posicionFinal,
                posicionInicial,
                suavizado
            );

            yield return null;
        }

        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = posicionFinal;

        currentCoroutine = null;
    }
}