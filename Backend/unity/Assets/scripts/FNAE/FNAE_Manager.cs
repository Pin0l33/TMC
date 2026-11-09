using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerFNAF : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoAbrirCamaras;
    public AudioClip sonidoCambiarCamara;
    public AudioClip sonidoBloqueo;

    [Header("UI")]
    public TMP_Text relojTexto;
    public TMP_Text energiaTexto;
    public TMP_Text usoTexto;

    [Header("Tiempo")]
    public int hora = 12;
    private float timerHora = 0f;
    public float segundosPorHora = 45f;

    [Header("Energia")]
    public float energia = 100f;

    [Header("Consumo")]
    public int uso = 1;

    public GameObject botonBloqueo;
    private TMP_Text bloqueoTexto;
    private Image bloqueoImagen;

    [Header("Bloqueo")]
    public bool bloqueoActivo = false;

    private bool juegoTerminado = false;

    void Awake()
    {
        FNAEProgression.EnsureInitialized();
        if (botonBloqueo != null)
        {
            bloqueoTexto = botonBloqueo.GetComponentInChildren<TMP_Text>(true);
            bloqueoImagen = botonBloqueo.GetComponent<Image>();
            ActualizarVisualBloqueo();
        }
    }

    void Update()
    {
        if (juegoTerminado || FNAEProgression.IsGameOver) return;

        ActualizarReloj();
        ConsumirEnergia();

        relojTexto.text = hora + " AM";
        energiaTexto.text = "ENERGÍA: " + Mathf.RoundToInt(energia) + "%";
        usoTexto.text = "CONSUMO: " + uso;
    }

    void ActualizarReloj()
    {
        timerHora += Time.deltaTime;

        if (timerHora >= segundosPorHora)
        {
            timerHora = 0f;
            hora++;

            if (hora == 6) GanarNoche();
            else if (hora == 13) hora = 1;
        }
    }

    void ConsumirEnergia()
    {
        energia -= uso * Time.deltaTime * 0.15f;

        if (energia <= 0)
        {
            energia = 0;
            GameOverEnergia();
        }
    }

    public void AbrirCamaras()
    {
        uso = bloqueoActivo ? 5 : 2;

        botonBloqueo.SetActive(false);

        if (audioSource != null && sonidoAbrirCamaras != null)
            audioSource.PlayOneShot(sonidoAbrirCamaras);
    }

    public void CerrarCamaras()
    {
        uso = bloqueoActivo ? 3 : 1;

        botonBloqueo.SetActive(true);

        if (audioSource != null && sonidoAbrirCamaras != null)
            audioSource.PlayOneShot(sonidoAbrirCamaras);
    }

    public void ToggleBloqueo()
    {
        bloqueoActivo = !bloqueoActivo;
        ActualizarVisualBloqueo();

        Debug.Log("Bloqueo: " + bloqueoActivo);

        if (audioSource != null && sonidoBloqueo != null)
        {
            audioSource.clip = sonidoBloqueo;
            audioSource.loop = bloqueoActivo;

            if (bloqueoActivo) audioSource.Play();
            else audioSource.Stop();
        }
    }

    private void ActualizarVisualBloqueo()
    {
        if (bloqueoTexto != null)
            bloqueoTexto.text = bloqueoActivo ? "BLOQUEO ACTIVO" : "BLOQUEO";
        if (bloqueoImagen != null)
            bloqueoImagen.color = bloqueoActivo
                ? new Color(0.32f, 0.075f, 0.07f, 1f)
                : new Color(0.09f, 0.17f, 0.19f, 1f);
    }

    void GanarNoche()
    {
        juegoTerminado = true;
        FNAEProgression.CompleteCurrentNight();
        SceneManager.LoadScene("Victory");
    }

    void GameOverEnergia()
    {
        juegoTerminado = true;
        FNAEProgression.ShowGameOver();
    }
}
