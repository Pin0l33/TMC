using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Profesor Celador advances through a separate camera route. At the office
/// warning, closing the tablet makes him retreat; the office lock does not.
/// </summary>
public sealed class FNAEProfesorCelador : MonoBehaviour
{
    public GameManagerFNAF gameManager;
    public CameraSystem cameraSystem;
    public TabletManager tabletManager;

    [Range(3, 5)] public int primeraNoche = 3;
    [Range(3, 5)] public int ultimaNoche = 5;
    public float segundosEntreCamaras = 38f;
    public float segundosParaCerrarTablet = 7f;
    public string[] rutaCamaras = { "Cam 1A", "Cam 2A", "Cam 3C" };

    private enum Estado { Avanzando, Ataque }
    private Estado estado;
    private int indiceCamara;
    private float temporizador;
    private float intervaloMovimiento;
    private float ventanaRespuesta;
    private Image retrato;
    private Image fondoAviso;
    private TMP_Text aviso;
    private Sprite retratoSprite;
    private AudioClip alarma;

    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManagerFNAF>();
        if (cameraSystem == null) cameraSystem = FindFirstObjectByType<CameraSystem>();
        if (tabletManager == null) tabletManager = FindFirstObjectByType<TabletManager>();

        int night = FNAEProgression.CurrentNight;
        if (night < primeraNoche || night > ultimaNoche || gameManager == null ||
            cameraSystem == null || cameraSystem.vistaCamara == null || rutaCamaras == null || rutaCamaras.Length == 0)
        {
            enabled = false;
            return;
        }

        float factor = FNAEProgression.GetEnemyTimeFactor();
        intervaloMovimiento = Mathf.Max(12f, segundosEntreCamaras * factor);
        ventanaRespuesta = Mathf.Max(4f, segundosParaCerrarTablet * factor);
        CrearPresentacion();
        alarma = gameManager.sonidoBloqueo;
    }

    private void Update()
    {
        if (FNAEProgression.IsGameOver || gameManager == null || cameraSystem == null)
            return;

        bool tabletAbierta = tabletManager != null && tabletManager.panelCamaras != null &&
                             tabletManager.panelCamaras.activeInHierarchy;
        string camaraActual = cameraSystem.nombreCamara != null ? cameraSystem.nombreCamara.text : string.Empty;
        bool visibleEnCamara = tabletAbierta && camaraActual == rutaCamaras[Mathf.Min(indiceCamara, rutaCamaras.Length - 1)];
        if (retrato != null)
            retrato.enabled = visibleEnCamara;

        if (estado == Estado.Ataque)
        {
            if (!tabletAbierta)
            {
                // Closing the monitor during the warning sends him back to his route.
                estado = Estado.Avanzando;
                indiceCamara = 0;
                temporizador = 0f;
                if (aviso != null) aviso.enabled = false;
                if (fondoAviso != null) fondoAviso.enabled = false;
                return;
            }

            temporizador += Time.deltaTime;
            if (temporizador >= ventanaRespuesta)
                FNAEProgression.ShowGameOver();
            return;
        }

        temporizador += Time.deltaTime;
        if (temporizador < intervaloMovimiento)
            return;

        temporizador = 0f;
        if (indiceCamara < rutaCamaras.Length - 1)
        {
            indiceCamara++;
            return;
        }

        estado = Estado.Ataque;
        temporizador = 0f;
        if (aviso != null) aviso.enabled = true;
        if (fondoAviso != null) fondoAviso.enabled = true;
        if (gameManager.audioSource != null && alarma != null)
            gameManager.audioSource.PlayOneShot(alarma);
    }

    private void CrearPresentacion()
    {
        retratoSprite = CrearRetratoPixelArt();
        var imageObject = new GameObject("Profesor Celador - retrato", typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(cameraSystem.vistaCamara.transform, false);
        var imageRect = imageObject.GetComponent<RectTransform>();
        imageRect.anchorMin = imageRect.anchorMax = new Vector2(0.5f, 0.5f);
        imageRect.pivot = new Vector2(0.5f, 0.5f);
        imageRect.anchoredPosition = new Vector2(0f, -20f);
        imageRect.sizeDelta = new Vector2(260f, 260f);
        retrato = imageObject.GetComponent<Image>();
        retrato.sprite = retratoSprite;
        retrato.preserveAspect = true;
        retrato.raycastTarget = false;
        retrato.enabled = false;

        Canvas canvas = cameraSystem.vistaCamara.canvas;
        if (canvas != null)
        {
            var noticeObject = new GameObject("Aviso Profesor Celador", typeof(RectTransform), typeof(Image));
            noticeObject.transform.SetParent(canvas.transform, false);
            var rect = noticeObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(-160f, 125f);
            rect.sizeDelta = new Vector2(920f, 72f);
            fondoAviso = noticeObject.GetComponent<Image>();
            fondoAviso.color = new Color(0.16f, 0.025f, 0.025f, 0.92f);
            fondoAviso.raycastTarget = false;
            fondoAviso.enabled = false;

            var labelObject = new GameObject("Texto Aviso Profesor Celador", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(noticeObject.transform, false);
            var labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 4f);
            labelRect.offsetMax = new Vector2(-16f, -4f);
            aviso = labelObject.GetComponent<TMP_Text>();
            aviso.text = "¡EL CELADOR VIENE! CERRÁ LA TABLET";
            aviso.alignment = TextAlignmentOptions.Center;
            aviso.fontSize = 30f;
            aviso.color = new Color(1f, 0.91f, 0.82f);
            aviso.textWrappingMode = TextWrappingModes.NoWrap;
            aviso.raycastTarget = false;
            aviso.enabled = false;
        }
    }

    private static Sprite CrearRetratoPixelArt()
    {
        const int size = 128;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.name = "Profesor Celador - retrato procedural";
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        var pixels = new Color[size * size];
        Color transparent = new Color(0f, 0f, 0f, 0f);
        Color outline = new Color(0.035f, 0.055f, 0.07f);
        Color skin = new Color(0.69f, 0.48f, 0.34f);
        Color hair = new Color(0.72f, 0.77f, 0.74f);
        Color coat = new Color(0.10f, 0.20f, 0.23f);
        Color shirt = new Color(0.79f, 0.81f, 0.72f);

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x - 63.5f;
            float dy = y - 68f;
            Color color = transparent;

            // shoulders, jacket and shirt
            if (y < 55 && y > 4 && Mathf.Abs(dx) < 48 - (4 - Mathf.Min(y, 4))) color = outline;
            if (y < 52 && y > 8 && Mathf.Abs(dx) < 43) color = coat;
            if (y < 47 && y > 9 && Mathf.Abs(dx) < 18) color = shirt;
            if (y < 34 && y > 10 && Mathf.Abs(dx) < 5) color = new Color(0.52f, 0.19f, 0.16f);

            // neck and face silhouette
            float faceX = dx / 27f;
            float faceY = (dy - 9f) / 34f;
            if (faceX * faceX + faceY * faceY < 1.08f) color = outline;
            if (faceX * faceX + faceY * faceY < 0.91f) color = skin;

            // swept silver hair and sideburns
            if (y > 84 && y < 111 && x > 34 && x < 94 &&
                (y < 99 + (int)(6f * Mathf.Sin(x * 0.12f)) || x < 43 || x > 85)) color = outline;
            if (y > 86 && y < 108 && x > 38 && x < 90 &&
                (y < 100 + (int)(6f * Mathf.Sin(x * 0.12f)) || x < 46 || x > 82)) color = hair;

            // square glasses, dark eyes, nose and mustache distinguish him from the visitor
            if (y > 69 && y < 82 && ((x > 41 && x < 59) || (x > 69 && x < 87)) &&
                (y == 69 || y == 81 || x == 41 || x == 58 || x == 70 || x == 86)) color = outline;
            if ((x == 50 || x == 78) && y == 75) color = new Color(0.10f, 0.12f, 0.13f);
            if (x > 62 && x < 67 && y > 60 && y < 72) color = new Color(0.46f, 0.30f, 0.24f);
            if (y > 56 && y < 62 && x > 53 && x < 76) color = new Color(0.15f, 0.12f, 0.12f);
            pixels[y * size + x] = color;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
