using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Diario : MonoBehaviour
{
    public Image imagenNegra; // fade
    public RectTransform imagenPeriodico; // zoom

    public float duracionFade = 3f;
    public float duracionTotal = 6f;
    public float zoomFinal = 1.2f;

    void Start()
    {
        StartCoroutine(Secuencia());
    }

    IEnumerator Secuencia()
    {
        
        imagenPeriodico.localScale = Vector3.one;

        // FADE IN (de negro a visible)
        yield return StartCoroutine(Fade(1f, 0f, duracionFade));

        
        float tiempo = 0f;
        while (tiempo < duracionTotal)
        {
            tiempo += Time.deltaTime;

            float t = tiempo / duracionTotal;
            float escala = Mathf.Lerp(1f, zoomFinal, t);
            imagenPeriodico.localScale = new Vector3(escala, escala, 1);

            yield return null;
        }

        
        yield return StartCoroutine(Fade(0f, 1f, duracionFade));

       
        SceneManager.LoadScene("TransicionDeNoche");
    }

    IEnumerator Fade(float inicio, float fin, float duracion)
    {
        float tiempo = 0f;
        Color color = imagenNegra.color;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(inicio, fin, tiempo / duracion);

            color.a = alpha;
            imagenNegra.color = color;

            yield return null;
        }
    }
}