using UnityEngine;
using UnityEngine.UI;

public class TVEffects : MonoBehaviour
{
    public Image imagen;
    public float velocidadCambio = 0.05f;
    void Start()
    {
        InvokeRepeating("CambiarRuido", 0, velocidadCambio);
    }

    void CambiarRuido()
    {
        float alpha = 0.1f + Random.Range(-0.05f, 0.05f);

        imagen.color = new Color(1, 1, 1, alpha);

        float scale = Random.Range(1f,1.1f);

        imagen.rectTransform.localScale = new Vector3(scale, scale, 1);
    }
    
    
}
