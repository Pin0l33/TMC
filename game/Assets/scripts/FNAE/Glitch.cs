using UnityEngine;

public class Glitch : MonoBehaviour
{
    public RectTransform rect;

    void Update()
    {
        if (Random.value < 0.02f)
        {
            float x = Random.Range(-10f, 10f);
            float y = Random.Range(-10f, 10f);
            rect.anchoredPosition = new Vector2(x, y);
        }
        else
        {
            rect.anchoredPosition = Vector2.zero;
        }
    }
}