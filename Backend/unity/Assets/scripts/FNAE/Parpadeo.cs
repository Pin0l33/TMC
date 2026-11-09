using UnityEngine;

public class Parpadeo : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float intensidad = 0.1f;

    void Update()
    {
        canvasGroup.alpha = 1f - Random.Range(0f, intensidad);
    }
}