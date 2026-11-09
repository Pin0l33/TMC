using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public enum CompatibilityLevel
{
    Compatible,
    Partial,
    NotCompatible
}

public struct MinigameCompatibilityInfo
{
    public string MinigameNameEs;
    public string MinigameNameEn;
    public CompatibilityLevel Level;
    public string ArduinoControlsEs;
    public string ArduinoControlsEn;
    public string OtherDevicesEs;
    public string OtherDevicesEn;
    public string ExplanationEs;
    public string ExplanationEn;
}

public class ArduinoCompatibilityPrompt : MonoBehaviour
{
    public static ArduinoCompatibilityPrompt Instance { get; private set; }

    private GameObject promptRoot;
    private TMP_Text titleText;
    private TMP_Text badgeText;
    private Image badgeBackground;
    private TMP_Text controlsText;
    private TMP_Text otherDevicesText;
    private TMP_Text explanationText;
    private Button continueButton;
    private Button backButton;
    private TMP_Text continueButtonText;
    private TMP_Text backButtonText;

    private Action pendingOnContinue;
    private Action pendingOnCancel;
    private bool isPromptActive;
    private int selectedButtonIndex = 0; // 0 = Continuar, 1 = Volver

    public bool IsPromptActive => isPromptActive;

    public static void EnsureInstance()
    {
        if (Instance != null) return;
        var existing = FindFirstObjectByType<ArduinoCompatibilityPrompt>();
        if (existing != null)
        {
            Instance = existing;
            return;
        }

        var go = new GameObject("ArduinoCompatibilityPrompt");
        Instance = go.AddComponent<ArduinoCompatibilityPrompt>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (transform.parent == null)
            DontDestroyOnLoad(gameObject);

        BuildPromptUI();
    }

    public static MinigameCompatibilityInfo GetInfoForTarget(string target)
    {
        if (string.IsNullOrEmpty(target))
        {
            return new MinigameCompatibilityInfo
            {
                MinigameNameEs = "Minijuego",
                MinigameNameEn = "Minigame",
                Level = CompatibilityLevel.Partial,
                ArduinoControlsEs = "Palanca y botón de interacción.",
                ArduinoControlsEn = "Stick and interaction button.",
                OtherDevicesEs = "Teclado o mouse según la escena.",
                OtherDevicesEn = "Keyboard or mouse depending on scene.",
                ExplanationEs = "Compatibilidad sujeta a verificación en ejecución.",
                ExplanationEn = "Compatibility subject to runtime verification."
            };
        }

        string clean = target.Trim().ToLowerInvariant();

        if (clean.Contains("taller"))
        {
            return new MinigameCompatibilityInfo
            {
                MinigameNameEs = "Taller Técnico: Calibración y Soldadura",
                MinigameNameEn = "Technical Workshop: Calibration & Soldering",
                Level = CompatibilityLevel.Compatible,
                ArduinoControlsEs = "Palanca analógica (calibración/movimiento) + Botón físico D3 (soldadura y confirmación) + Botón D2 (pausa).",
                ArduinoControlsEn = "Analog stick (calibration/movement) + Physical button D3 (soldering & confirm) + D2 button (pause).",
                OtherDevicesEs = "Ninguno obligatorio. 100% jugable con el joystick físico de ExpoTécnica.",
                OtherDevicesEn = "None required. 100% playable with ExpoTécnica physical joystick.",
                ExplanationEs = "Minijuego oficial de la ExpoTécnica diseñado para completarse íntegramente desde el mueble arcade / Arduino.",
                ExplanationEn = "Official ExpoTécnica minigame designed to be completed fully from the physical arcade / Arduino."
            };
        }

        if (clean.Contains("boss") || clean.Contains("battle") || clean.Contains("guardian"))
        {
            return new MinigameCompatibilityInfo
            {
                MinigameNameEs = "Combate contra Guardián (Boss Fight)",
                MinigameNameEn = "Guardian Battle (Boss Fight)",
                Level = CompatibilityLevel.Compatible,
                ArduinoControlsEs = "Palanca analógica (esquivar en caja y navegar opciones) + Botón físico D3 (golpe rítmico y selección).",
                ArduinoControlsEn = "Analog stick (dodge in box & navigate options) + Physical button D3 (rhythm strike & select).",
                OtherDevicesEs = "Ninguno obligatorio. Teclado disponible como soporte alternativo (WASD/Z).",
                OtherDevicesEn = "None required. Keyboard available as backup (WASD/Z).",
                ExplanationEs = "El minijuego estilo Undertale es completamente compatible con el control y botón de Arduino.",
                ExplanationEn = "The Undertale-style minigame is completely compatible with Arduino joystick and button."
            };
        }

        if (clean.Contains("fnae") || clean.Contains("diario") || clean.Contains("noche") || clean.Contains("victory") || clean.Contains("lemos"))
        {
            return new MinigameCompatibilityInfo
            {
                MinigameNameEs = "FNAE: Five Nights at Epifanio",
                MinigameNameEn = "FNAE: Five Nights at Epifanio",
                Level = CompatibilityLevel.Partial,
                ArduinoControlsEs = "Botón físico D3 (saltar cinemática del periódico y volver al menú).",
                ArduinoControlsEn = "Physical button D3 (skip newspaper cutscene & return to menu).",
                OtherDevicesEs = "MOUSE OBLIGATORIO: requerido para abrir la tablet, alternar cámaras, operar el bloqueo de puerta y mover la vista de la oficina.",
                OtherDevicesEn = "MOUSE REQUIRED: needed to open tablet, switch cameras, toggle door lock, and pan office view.",
                ExplanationEs = "El minijuego de vigilancia escolar requiere interacción táctil/puntero de mouse para el monitoreo de cámaras.",
                ExplanationEn = "The surveillance minigame requires mouse pointer interaction for the security camera monitoring system."
            };
        }

        if (clean.Contains("galeria"))
        {
            return new MinigameCompatibilityInfo
            {
                MinigameNameEs = "Galería Escolar",
                MinigameNameEn = "School Gallery",
                Level = CompatibilityLevel.Compatible,
                ArduinoControlsEs = "Palanca analógica (caminar) + Botón físico D3 (entrar por puertas).",
                ArduinoControlsEn = "Analog stick (walk) + Physical button D3 (door entry).",
                OtherDevicesEs = "Ninguno obligatorio.",
                OtherDevicesEn = "None required.",
                ExplanationEs = "Área de exposición escolar de arte, completamente navegable con el joystick Arduino.",
                ExplanationEn = "School art exhibition area, fully navigable with Arduino joystick."
            };
        }

        if (clean.Contains("kiosco"))
        {
            return new MinigameCompatibilityInfo
            {
                MinigameNameEs = "Kiosco Escolar",
                MinigameNameEn = "School Kiosk",
                Level = CompatibilityLevel.NotCompatible,
                ArduinoControlsEs = "Sin controles asignados.",
                ArduinoControlsEn = "No controls assigned.",
                OtherDevicesEs = "Escena prototipo en desarrollo.",
                OtherDevicesEn = "Prototype scene in development.",
                ExplanationEs = "Escena sin mecánicas de juego activas en esta versión.",
                ExplanationEn = "Scene without active gameplay mechanics in this build."
            };
        }

        return new MinigameCompatibilityInfo
        {
            MinigameNameEs = "Zona: " + target,
            MinigameNameEn = "Zone: " + target,
            Level = CompatibilityLevel.Compatible,
            ArduinoControlsEs = "Palanca analógica + Botón físico D3.",
            ArduinoControlsEn = "Analog stick + Physical button D3.",
            OtherDevicesEs = "Ninguno.",
            OtherDevicesEn = "None.",
            ExplanationEs = "Área compatible con joystick físico Arduino.",
            ExplanationEn = "Area compatible with Arduino physical joystick."
        };
    }

    public static void Show(string targetIdentifier, Action onContinue, Action onCancel = null)
    {
        EnsureInstance();
        Instance.Open(targetIdentifier, onContinue, onCancel);
    }

    public void Open(string targetIdentifier, Action onContinue, Action onCancel)
    {
        pendingOnContinue = onContinue;
        pendingOnCancel = onCancel;

        MinigameCompatibilityInfo info = GetInfoForTarget(targetIdentifier);
        ApplyInfo(info);

        MovimientoJugador.canMove = false;
        selectedButtonIndex = 0;
        isPromptActive = true;
        promptRoot.SetActive(true);
        UpdateButtonVisuals();
    }

    private void ApplyInfo(MinigameCompatibilityInfo info)
    {
        bool es = LocalizationManager.IsSpanish;

        titleText.text = es ? info.MinigameNameEs : info.MinigameNameEn;

        switch (info.Level)
        {
            case CompatibilityLevel.Compatible:
                badgeText.text = es ? "COMPATIBLE CON ARDUINO" : "ARDUINO COMPATIBLE";
                badgeBackground.color = new Color(0.12f, 0.65f, 0.35f, 0.95f); // Green
                break;
            case CompatibilityLevel.Partial:
                badgeText.text = es ? "COMPATIBILIDAD PARCIAL" : "PARTIAL COMPATIBILITY";
                badgeBackground.color = new Color(0.85f, 0.55f, 0.1f, 0.95f); // Amber / Orange
                break;
            case CompatibilityLevel.NotCompatible:
                badgeText.text = es ? "NO COMPATIBLE CON ARDUINO" : "NOT COMPATIBLE WITH ARDUINO";
                badgeBackground.color = new Color(0.8f, 0.18f, 0.18f, 0.95f); // Red
                break;
        }

        controlsText.text = "<b>" + (es ? "Controles Arduino: " : "Arduino Controls: ") + "</b>" +
                            (es ? info.ArduinoControlsEs : info.ArduinoControlsEn);

        otherDevicesText.text = "<b>" + (es ? "Dispositivos adicionales: " : "Additional Devices: ") + "</b>" +
                                (es ? info.OtherDevicesEs : info.OtherDevicesEn);

        explanationText.text = es ? info.ExplanationEs : info.ExplanationEn;

        continueButtonText.text = es ? "CONTINUAR [E / D3]" : "CONTINUE [E / D3]";
        backButtonText.text = es ? "VOLVER" : "BACK";
    }

    private void Update()
    {
        if (!isPromptActive || promptRoot == null || !promptRoot.activeSelf) return;

        // Navegación por palanca analógica o flechas/WASD
        bool left = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || ArduinoJoystickInput.ConsumeMenuLeft();
        bool right = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || ArduinoJoystickInput.ConsumeMenuRight();
        bool up = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || ArduinoJoystickInput.ConsumeMenuUp();
        bool down = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || ArduinoJoystickInput.ConsumeMenuDown();

        if (left || up)
        {
            selectedButtonIndex = 0;
            UpdateButtonVisuals();
        }
        else if (right || down)
        {
            selectedButtonIndex = 1;
            UpdateButtonVisuals();
        }

        if (EntradaJuego.GetInteractDown() || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedButtonIndex == 0)
                OnContinueClicked();
            else
                OnBackClicked();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackClicked();
        }
    }

    private void UpdateButtonVisuals()
    {
        if (continueButton == null || backButton == null) return;

        var contImg = continueButton.GetComponent<Image>();
        var backImg = backButton.GetComponent<Image>();

        if (contImg != null)
            contImg.color = selectedButtonIndex == 0 ? new Color(0.18f, 0.5f, 0.9f, 1f) : new Color(0.2f, 0.2f, 0.25f, 0.9f);

        if (backImg != null)
            backImg.color = selectedButtonIndex == 1 ? new Color(0.85f, 0.3f, 0.3f, 1f) : new Color(0.2f, 0.2f, 0.25f, 0.9f);
    }

    public void OnContinueClicked()
    {
        Close();
        var action = pendingOnContinue;
        pendingOnContinue = null;
        pendingOnCancel = null;
        action?.Invoke();
    }

    public void OnBackClicked()
    {
        Close();
        MovimientoJugador.canMove = true;
        var action = pendingOnCancel;
        pendingOnContinue = null;
        pendingOnCancel = null;
        action?.Invoke();
    }

    private void Close()
    {
        isPromptActive = false;
        if (promptRoot != null)
            promptRoot.SetActive(false);
    }

    private void BuildPromptUI()
    {
        // Crear Canvas dedicado persistente para garantizar visibilidad en cualquier resolución
        promptRoot = new GameObject("CompatibilityPromptCanvas");
        promptRoot.transform.SetParent(transform, false);

        var canvas = promptRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Siempre sobre el resto de las interfaces

        var scaler = promptRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        promptRoot.AddComponent<GraphicRaycaster>();

        // Fondo oscuro bloqueante
        var overlayObj = new GameObject("Backdrop", typeof(Image));
        overlayObj.transform.SetParent(promptRoot.transform, false);
        var overlayRect = overlayObj.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayObj.GetComponent<Image>().color = new Color(0.04f, 0.05f, 0.08f, 0.88f);

        // Tarjeta modal central
        var panelObj = new GameObject("ModalCard", typeof(Image));
        panelObj.transform.SetParent(promptRoot.transform, false);
        var panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(860, 560);
        var panelImg = panelObj.GetComponent<Image>();
        panelImg.color = new Color(0.1f, 0.12f, 0.16f, 0.98f);

        // Encabezado
        var titleObj = new GameObject("TitleText", typeof(TextMeshProUGUI));
        titleObj.transform.SetParent(panelObj.transform, false);
        var titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 215);
        titleRect.sizeDelta = new Vector2(800, 60);
        titleText = titleObj.GetComponent<TextMeshProUGUI>();
        titleText.fontSize = 32;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;

        // Badge de Compatibilidad
        var badgeObj = new GameObject("BadgeBackground", typeof(Image));
        badgeObj.transform.SetParent(panelObj.transform, false);
        var badgeRect = badgeObj.GetComponent<RectTransform>();
        badgeRect.anchoredPosition = new Vector2(0, 150);
        badgeRect.sizeDelta = new Vector2(480, 42);
        badgeBackground = badgeObj.GetComponent<Image>();
        badgeBackground.color = new Color(0.12f, 0.65f, 0.35f, 0.95f);

        var badgeTextObj = new GameObject("BadgeText", typeof(TextMeshProUGUI));
        badgeTextObj.transform.SetParent(badgeObj.transform, false);
        var badgeTextRect = badgeTextObj.GetComponent<RectTransform>();
        badgeTextRect.sizeDelta = new Vector2(460, 36);
        badgeText = badgeTextObj.GetComponent<TextMeshProUGUI>();
        badgeText.fontSize = 22;
        badgeText.fontStyle = FontStyles.Bold;
        badgeText.alignment = TextAlignmentOptions.Center;
        badgeText.color = Color.white;

        // Texto Controles Arduino
        var ctrlObj = new GameObject("ArduinoControlsText", typeof(TextMeshProUGUI));
        ctrlObj.transform.SetParent(panelObj.transform, false);
        var ctrlRect = ctrlObj.GetComponent<RectTransform>();
        ctrlRect.anchoredPosition = new Vector2(0, 75);
        ctrlRect.sizeDelta = new Vector2(760, 70);
        controlsText = ctrlObj.GetComponent<TextMeshProUGUI>();
        controlsText.fontSize = 20;
        controlsText.alignment = TextAlignmentOptions.TopLeft;
        controlsText.color = new Color(0.9f, 0.95f, 1f);

        // Texto Otros Dispositivos
        var otherObj = new GameObject("OtherDevicesText", typeof(TextMeshProUGUI));
        otherObj.transform.SetParent(panelObj.transform, false);
        var otherRect = otherObj.GetComponent<RectTransform>();
        otherRect.anchoredPosition = new Vector2(0, -10);
        otherRect.sizeDelta = new Vector2(760, 70);
        otherDevicesText = otherObj.GetComponent<TextMeshProUGUI>();
        otherDevicesText.fontSize = 20;
        otherDevicesText.alignment = TextAlignmentOptions.TopLeft;
        otherDevicesText.color = new Color(1f, 0.85f, 0.7f);

        // Texto Explicación
        var expObj = new GameObject("ExplanationText", typeof(TextMeshProUGUI));
        expObj.transform.SetParent(panelObj.transform, false);
        var expRect = expObj.GetComponent<RectTransform>();
        expRect.anchoredPosition = new Vector2(0, -95);
        expRect.sizeDelta = new Vector2(760, 65);
        explanationText = expObj.GetComponent<TextMeshProUGUI>();
        explanationText.fontSize = 18;
        explanationText.fontStyle = FontStyles.Italic;
        explanationText.alignment = TextAlignmentOptions.TopLeft;
        explanationText.color = new Color(0.8f, 0.82f, 0.86f);

        // Botón Continuar
        var btnContObj = new GameObject("ButtonContinue", typeof(Image), typeof(Button));
        btnContObj.transform.SetParent(panelObj.transform, false);
        var btnContRect = btnContObj.GetComponent<RectTransform>();
        btnContRect.anchoredPosition = new Vector2(-170, -200);
        btnContRect.sizeDelta = new Vector2(280, 56);
        continueButton = btnContObj.GetComponent<Button>();
        continueButton.onClick.AddListener(OnContinueClicked);

        var btnContTxtObj = new GameObject("BtnContText", typeof(TextMeshProUGUI));
        btnContTxtObj.transform.SetParent(btnContObj.transform, false);
        var btnContTxtRect = btnContTxtObj.GetComponent<RectTransform>();
        btnContTxtRect.sizeDelta = btnContRect.sizeDelta;
        continueButtonText = btnContTxtObj.GetComponent<TextMeshProUGUI>();
        continueButtonText.fontSize = 20;
        continueButtonText.fontStyle = FontStyles.Bold;
        continueButtonText.alignment = TextAlignmentOptions.Center;
        continueButtonText.color = Color.white;

        // Botón Volver
        var btnBackObj = new GameObject("ButtonBack", typeof(Image), typeof(Button));
        btnBackObj.transform.SetParent(panelObj.transform, false);
        var btnBackRect = btnBackObj.GetComponent<RectTransform>();
        btnBackRect.anchoredPosition = new Vector2(170, -200);
        btnBackRect.sizeDelta = new Vector2(280, 56);
        backButton = btnBackObj.GetComponent<Button>();
        backButton.onClick.AddListener(OnBackClicked);

        var btnBackTxtObj = new GameObject("BtnBackText", typeof(TextMeshProUGUI));
        btnBackTxtObj.transform.SetParent(btnBackObj.transform, false);
        var btnBackTxtRect = btnBackTxtObj.GetComponent<RectTransform>();
        btnBackTxtRect.sizeDelta = btnBackRect.sizeDelta;
        backButtonText = btnBackTxtObj.GetComponent<TextMeshProUGUI>();
        backButtonText.fontSize = 20;
        backButtonText.fontStyle = FontStyles.Bold;
        backButtonText.alignment = TextAlignmentOptions.Center;
        backButtonText.color = Color.white;

        promptRoot.SetActive(false);
    }
}
