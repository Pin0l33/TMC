using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Bloqueo")]
    public bool bloqueoActivo = false;

    private bool juegoTerminado = false;

    void Update()
    {
        if (juegoTerminado) return;

        ActualizarReloj();
        ConsumirEnergia();

        relojTexto.text = hora + " AM";
        energiaTexto.text = "Power: " + Mathf.RoundToInt(energia) + "%";
        usoTexto.text = "Usage: " + uso;
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

        Debug.Log("Bloqueo: " + bloqueoActivo);

        if (audioSource != null && sonidoBloqueo != null)
        {
            audioSource.clip = sonidoBloqueo;
            audioSource.loop = bloqueoActivo;

            if (bloqueoActivo) audioSource.Play();
            else audioSource.Stop();
        }
    }

    void GanarNoche()
    {
        juegoTerminado = true;
        SceneManager.LoadScene("Victory");
    }

    void GameOverEnergia()
    {
        juegoTerminado = true;
        SceneManager.LoadScene("GameOverScene");
    }
}