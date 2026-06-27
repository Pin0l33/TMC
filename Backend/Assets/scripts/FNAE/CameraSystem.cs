using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraSystem : MonoBehaviour
{
    public GameManagerFNAF gameManager;

    public Image vistaCamara;
    public TMP_Text nombreCamara;

    [Header("Sprites cámaras")]
    public Sprite cam1A;
    public Sprite cam1B;
    public Sprite cam1C;

    public Sprite cam2A;
    public Sprite cam2B;

    public Sprite cam3A;
    public Sprite cam3B;
    public Sprite cam3C;

    public void CambiarCamara(string nombre)
    {
        nombreCamara.text = nombre;

        // 🔊 sonido cambio cámara
        if (gameManager != null &&
            gameManager.audioSource != null &&
            gameManager.sonidoCambiarCamara != null)
        {
            gameManager.audioSource.PlayOneShot(gameManager.sonidoCambiarCamara);
        }

        switch (nombre)
        {
            case "Cam 1A": vistaCamara.sprite = cam1A; break;
            case "Cam 1B": vistaCamara.sprite = cam1B; break;
            case "Cam 1C": vistaCamara.sprite = cam1C; break;

            case "Cam 2A": vistaCamara.sprite = cam2A; break;
            case "Cam 2B": vistaCamara.sprite = cam2B; break;

            case "Cam 3A": vistaCamara.sprite = cam3A; break;
            case "Cam 3B": vistaCamara.sprite = cam3B; break;
            case "Cam 3C": vistaCamara.sprite = cam3C; break;
        }
    }
}