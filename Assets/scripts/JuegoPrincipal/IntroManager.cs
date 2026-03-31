using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class IntroFlexible : MonoBehaviour
{
    [Header("Textos y/o imágenes")]
    public CanvasGroup[] elementos; 
    public float fadeInTime = 1f;
    public float stayTime = 2f;
    public float fadeOutTime = 1f;

    [Header("Transición a escena siguiente")]
    public float fadeTransitionTime = 1f;
    public string escenaSiguiente = "MenuPrincipal";

    [Header("Música de intro (opcional)")]
    public AudioSource musicaFondo;

    void Start()
    {
        
        foreach (CanvasGroup elem in elementos)
        {
            elem.alpha = 0f;
            elem.gameObject.SetActive(true); 
        }

        if (musicaFondo != null)
            musicaFondo.Play();

        StartCoroutine(MostrarElementos());
    }

    IEnumerator MostrarElementos()
    {
        foreach (CanvasGroup elem in elementos)
        {
            // Fade in
            float t = 0f;
            while (t < fadeInTime)
            {
                t += Time.deltaTime;
                elem.alpha = Mathf.Clamp01(t / fadeInTime);
                yield return null;
            }
            elem.alpha = 1f;

            
            yield return new WaitForSeconds(stayTime);

            
            t = 0f;
            while (t < fadeOutTime)
            {
                t += Time.deltaTime;
                elem.alpha = Mathf.Clamp01(1f - t / fadeOutTime);
                yield return null;
            }
            elem.alpha = 0f;
        }

        
        StartCoroutine(FadeANegro());
    }

    IEnumerator FadeANegro()
    {
        GameObject panelNegro = new GameObject("PanelNegro");
        panelNegro.transform.SetParent(transform, false);

        Canvas canvas = panelNegro.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = panelNegro.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        panelNegro.AddComponent<GraphicRaycaster>();

        Image img = panelNegro.AddComponent<Image>();
        img.color = Color.black;

        CanvasGroup cg = panelNegro.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        float t = 0f;
        while (t < fadeTransitionTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(t / fadeTransitionTime);
            yield return null;
        }
        cg.alpha = 1f;

        SceneManager.LoadScene(escenaSiguiente);
    }
}