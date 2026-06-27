using UnityEngine;

public class CamaraSeguir : MonoBehaviour
{
    public Transform jugador;
    public float velocidad = 5f;

    void LateUpdate()
    {
       
        if (jugador == null) return;

        Vector3 posicionDelJugador = new Vector3(
            jugador.position.x,
            jugador.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            posicionDelJugador,
            velocidad * Time.deltaTime
        );
    }
}