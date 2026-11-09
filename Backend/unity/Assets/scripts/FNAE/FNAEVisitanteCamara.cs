using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The white-eyed figure from the FNAE menu moves through the camera network,
/// pauses when watched, and must be stopped with the existing office lock.
/// </summary>
public sealed class FNAEVisitanteCamara : MonoBehaviour
{
    [Header("Scene references")]
    public GameManagerFNAF gameManager;
    public CameraSystem cameraSystem;
    public TabletManager tabletManager;
    public Sprite spriteProfesor;
    public AudioClip sonidoAtaque;

    [Header("Night schedule")]
    [Range(1, 5)] public int primeraNoche = 2;
    [Range(1, 5)] public int ultimaNoche = 5;

    [Header("Base timing; progression scales both values")]
    public float segundosEntreCamaras = 24f;
    public float segundosParaBloquear = 5f;

    [Header("Camera route")]
    public string[] rutaCamaras = { "Cam 1B", "Cam 1C", "Cam 2A", "Cam 2B" };

    private enum Estado { Avanzando, Ataque, Bloqueado }
    private Estado estado;
    private int indiceCamara;
    private float temporizador;
    private float intervaloMovimiento;
    private float ventanaBloqueo;
    private Image imagenEnemigo;
    private GameObject panelAviso;

    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManagerFNAF>();
        if (cameraSystem == null) cameraSystem = FindFirstObjectByType<CameraSystem>();
        if (tabletManager == null) tabletManager = FindFirstObjectByType<TabletManager>();

        int noche = FNAEProgression.CurrentNight;
        if (noche < primeraNoche || noche > ultimaNoche || rutaCamaras == null || rutaCamaras.Length == 0)
        {
            enabled = false;
            return;
        }

        float factor = FNAEProgression.GetEnemyTimeFactor();
        intervaloMovimiento = Mathf.Max(5f, segundosEntreCamaras * factor);
        ventanaBloqueo = Mathf.Max(1.5f, segundosParaBloquear * factor);
        estado = Estado.Avanzando;
        CrearImagenDeCamara();
        CrearAvisoDeAtaque();
    }

    private void Update()
    {
        if (gameManager == null || cameraSystem == null || FNAEProgression.IsGameOver)
            return;

        bool camarasAbiertas = tabletManager != null && tabletManager.panelCamaras != null &&
                               tabletManager.panelCamaras.activeInHierarchy;
        string camaraActual = cameraSystem.nombreCamara != null ? cameraSystem.nombreCamara.text : string.Empty;
        string camaraEnemigo = rutaCamaras[Mathf.Min(indiceCamara, rutaCamaras.Length - 1)];
        bool jugadorLoObserva = camarasAbiertas && camaraActual == camaraEnemigo;

        if (imagenEnemigo != null)
            imagenEnemigo.enabled = jugadorLoObserva && estado != Estado.Bloqueado;

        if (estado == Estado.Ataque)
        {
            if (gameManager.bloqueoActivo)
            {
                estado = Estado.Bloqueado;
                temporizador = 0f;
                indiceCamara = 0;
                if (panelAviso != null) panelAviso.SetActive(false);
                estado = Estado.Avanzando;
            }
            else
            {
                temporizador += Time.deltaTime;
                if (temporizador >= ventanaBloqueo)
                {
                    estado = Estado.Bloqueado;
                    FNAEProgression.ShowGameOver();
                }
            }
            return;
        }

        // Watching the exact room reveals the figure and slows its approach.
        temporizador += Time.deltaTime * (jugadorLoObserva ? 0.25f : 1f);
        if (temporizador < intervaloMovimiento)
            return;

        temporizador = 0f;
        if (indiceCamara < rutaCamaras.Length - 1)
        {
            indiceCamara++;
            return;
        }

        estado = Estado.Ataque;
        if (panelAviso != null) panelAviso.SetActive(true);
        if (gameManager.audioSource != null && sonidoAtaque != null)
            gameManager.audioSource.PlayOneShot(sonidoAtaque);
    }

    private void CrearImagenDeCamara()
    {
        if (cameraSystem.vistaCamara == null || spriteProfesor == null)
            return;

        var go = new GameObject("Figura de ojos blancos", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(cameraSystem.vistaCamara.transform, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -20f);
        rect.sizeDelta = new Vector2(260f, 260f);

        imagenEnemigo = go.GetComponent<Image>();
        imagenEnemigo.sprite = spriteProfesor;
        imagenEnemigo.preserveAspect = true;
        imagenEnemigo.raycastTarget = false;
        imagenEnemigo.enabled = false;
    }

    private void CrearAvisoDeAtaque()
    {
        Canvas canvas = cameraSystem.vistaCamara.canvas;
        if (canvas == null)
            return;

        panelAviso = new GameObject("Aviso Visitante", typeof(RectTransform), typeof(Image));
        panelAviso.transform.SetParent(canvas.transform, false);
        var rect = panelAviso.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(-160f, 220f);
        rect.sizeDelta = new Vector2(920f, 72f);

        var background = panelAviso.GetComponent<Image>();
        background.color = new Color(0.16f, 0.025f, 0.025f, 0.92f);
        background.raycastTarget = false;

        var labelObject = new GameObject("Texto Aviso Visitante", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(panelAviso.transform, false);
        var labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(16f, 4f);
        labelRect.offsetMax = new Vector2(-16f, -4f);
        var label = labelObject.GetComponent<TMP_Text>();
        label.text = "¡VISITANTE! CERRÁ LA TABLET Y ACTIVÁ EL BLOQUEO";
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 30f;
        label.color = new Color(1f, 0.91f, 0.82f);
        label.textWrappingMode = TextWrappingModes.NoWrap;
        label.raycastTarget = false;
        panelAviso.SetActive(false);
    }
}
