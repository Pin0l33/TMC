using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    [SerializeField] private string nombreZona;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Jugador"))
            return;

        if (ZoneBanner.Instance != null)
        {
            ZoneBanner.Instance.MostrarZona(nombreZona);
        }
    }
}