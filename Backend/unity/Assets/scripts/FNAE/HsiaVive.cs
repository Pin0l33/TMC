using UnityEngine;

public class MovimientoSutil : MonoBehaviour
{
    public float intensidad = 2f;
    public float velocidad = 1f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * velocidad) * intensidad;
        transform.localPosition = posicionInicial + new Vector3(x, 0, 0);
    }
}