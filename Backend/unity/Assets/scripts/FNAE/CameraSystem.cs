using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [Header("Monitor de vigilancia")]
    public Sprite texturaInterferencia;
    [Range(0.05f, 0.3f)] public float duracionInterferencia = 0.16f;
    [Range(0.03f, 0.2f)] public float opacidadInterferencia = 0.09f;

    private Image imagenInterferencia;
    private Coroutine rutinaInterferencia;

    private void Start()
    {
        if (vistaCamara != null && vistaCamara.sprite == cam1C)
            SeleccionarBotonCamara("Cam 1C");
        if (nombreCamara != null && string.IsNullOrWhiteSpace(nombreCamara.text))
            nombreCamara.text = "Cam 1C";

        CrearEstadoMonitor();
        CrearInterferencia();
    }

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

        SeleccionarBotonCamara(nombre);
        IniciarInterferencia();
    }

    private void SeleccionarBotonCamara(string nombre)
    {
        var botones = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < botones.Length; i++)
        {
            Button boton = botones[i];
            if (!boton.name.StartsWith("Cam ")) continue;

            ColorBlock colors = boton.colors;
            bool selected = boton.name == nombre;
            colors.normalColor = selected ? new Color(0.20f, 0.39f, 0.28f, 1f) : new Color(0.15f, 0.23f, 0.21f, 1f);
            colors.highlightedColor = selected ? new Color(0.29f, 0.50f, 0.36f, 1f) : new Color(0.24f, 0.34f, 0.29f, 1f);
            colors.pressedColor = new Color(0.37f, 0.20f, 0.18f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.fadeDuration = 0.08f;
            boton.colors = colors;

            TMP_Text etiqueta = boton.GetComponentInChildren<TMP_Text>(true);
            if (etiqueta != null)
                etiqueta.color = new Color(0.84f, 0.88f, 0.80f, 1f);
        }
    }

    private void CrearEstadoMonitor()
    {
        if (vistaCamara == null || vistaCamara.canvas == null || vistaCamara.transform.Find("Estado monitor") != null)
            return;

        var panel = new GameObject("Estado monitor", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(vistaCamara.transform, false);

        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-330f, -24f);
        rect.sizeDelta = new Vector2(310f, 46f);

        var background = panel.GetComponent<Image>();
        background.color = new Color(0.018f, 0.045f, 0.038f, 0.92f);
        background.raycastTarget = false;

        var ledObject = new GameObject("REC", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        ledObject.transform.SetParent(panel.transform, false);
        var ledRect = ledObject.GetComponent<RectTransform>();
        ledRect.anchorMin = ledRect.anchorMax = new Vector2(0f, 0.5f);
        ledRect.pivot = new Vector2(0f, 0.5f);
        ledRect.anchoredPosition = new Vector2(15f, 0f);
        ledRect.sizeDelta = new Vector2(12f, 12f);

        var led = ledObject.GetComponent<Image>();
        led.sprite = ObtenerSpriteBotonCamara();
        led.color = new Color(0.64f, 0.17f, 0.14f, 1f);
        led.raycastTarget = false;

        var labelObject = new GameObject("Texto estado monitor", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(panel.transform, false);
        var labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(38f, 3f);
        labelRect.offsetMax = new Vector2(-8f, -3f);

        var label = labelObject.GetComponent<TMP_Text>();
        label.text = "REC  |  SEÑAL ESTABLE";
        label.font = nombreCamara != null ? nombreCamara.font : null;
        label.fontSize = 22f;
        label.color = new Color(0.72f, 0.82f, 0.72f, 1f);
        label.alignment = TextAlignmentOptions.Left;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        label.raycastTarget = false;
    }

    private Sprite ObtenerSpriteBotonCamara()
    {
        var botones = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < botones.Length; i++)
        {
            if (!botones[i].name.StartsWith("Cam ")) continue;
            var image = botones[i].GetComponent<Image>();
            if (image != null && image.sprite != null) return image.sprite;
        }
        return null;
    }

    private void CrearInterferencia()
    {
        if (vistaCamara == null || texturaInterferencia == null || imagenInterferencia != null)
            return;

        var overlay = new GameObject("Interferencia breve", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        overlay.transform.SetParent(vistaCamara.transform, false);

        var rect = overlay.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        imagenInterferencia = overlay.GetComponent<Image>();
        imagenInterferencia.sprite = texturaInterferencia;
        imagenInterferencia.color = new Color(1f, 1f, 1f, opacidadInterferencia);
        imagenInterferencia.raycastTarget = false;
        imagenInterferencia.enabled = false;
    }

    private void IniciarInterferencia()
    {
        if (imagenInterferencia == null) return;
        if (rutinaInterferencia != null) StopCoroutine(rutinaInterferencia);
        rutinaInterferencia = StartCoroutine(OcultarInterferencia());
    }

    private IEnumerator OcultarInterferencia()
    {
        imagenInterferencia.enabled = true;
        yield return new WaitForSecondsRealtime(duracionInterferencia);
        if (imagenInterferencia != null) imagenInterferencia.enabled = false;
        rutinaInterferencia = null;
    }
}
