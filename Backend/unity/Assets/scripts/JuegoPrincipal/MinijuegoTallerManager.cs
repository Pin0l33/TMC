using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum TallerGameState
{
    Instructions,
    Phase1Calibration,
    Phase2Soldering,
    Victory,
    GameOver
}

public class MinijuegoTallerManager : MonoBehaviour
{
    public static MinijuegoTallerManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitOnSceneLoad()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        CheckCurrentScene(SceneManager.GetActiveScene());
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckCurrentScene(scene);
    }

    private static void CheckCurrentScene(Scene scene)
    {
        if (scene.name == "Taller")
        {
            if (Instance == null)
            {
                var go = new GameObject("MinijuegoTallerController");
                go.AddComponent<MinijuegoTallerManager>();
            }
        }
    }

    [Header("Configuración de Tiempo")]
    public float maxTimeSeconds = 80f;
    private float currentTimer;

    [Header("Fase 1: Calibración")]
    public float calibrationDurationRequired = 12f;
    private float calibrationCurrentProgress = 0f;
    private float targetZoneCenter = 0.5f;
    private float targetZoneWidth = 0.24f;
    private float probePosition = 0.5f;
    private float targetMoveSpeed = 0.4f;
    private float targetMoveDirection = 1f;

    [Header("Fase 2: Soldadura")]
    public int requiredWelds = 5;
    private int successfulWelds = 0;
    private float solderMarkerPosition = 0f;
    private float solderMarkerSpeed = 1.3f;
    private float solderTargetMin = 0.40f;
    private float solderTargetMax = 0.60f;

    private int score = 0;
    private TallerGameState state = TallerGameState.Instructions;

    // UI Elements
    private GameObject canvasRoot;
    private TMP_Text headerTitle;
    private TMP_Text instructionText;
    private TMP_Text timerText;
    private TMP_Text scoreText;
    private TMP_Text phaseStatusText;

    // Phase 1 UI
    private GameObject phase1Container;
    private RectTransform targetZoneRect;
    private RectTransform probeIndicatorRect;
    private Image calibrationProgressBar;

    // Phase 2 UI
    private GameObject phase2Container;
    private RectTransform solderTrackRect;
    private RectTransform solderTargetZoneRect;
    private RectTransform solderMarkerRect;
    private TMP_Text weldCountText;
    private TMP_Text weldFeedbackText;
    private float feedbackTimer = 0f;

    // End Game UI
    private GameObject endGameContainer;
    private TMP_Text endTitleText;
    private TMP_Text endScoreText;
    private Button retryButton;
    private Button exitToMapButton;
    private TMP_Text retryButtonText;
    private TMP_Text exitButtonText;
    private int selectedEndOption = 0; // 0 = Retry, 1 = Exit

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        BuildWorkshopUI();
    }

    private void Start()
    {
        currentTimer = maxTimeSeconds;
        state = TallerGameState.Instructions;
        ShowInstructions();
    }

    private void Update()
    {
        switch (state)
        {
            case TallerGameState.Instructions:
                UpdateInstructions();
                break;
            case TallerGameState.Phase1Calibration:
                UpdateCalibration();
                break;
            case TallerGameState.Phase2Soldering:
                UpdateSoldering();
                break;
            case TallerGameState.Victory:
            case TallerGameState.GameOver:
                UpdateEndMenu();
                break;
        }

        if (feedbackTimer > 0f)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0f && weldFeedbackText != null)
                weldFeedbackText.text = "";
        }
    }

    private void UpdateInstructions()
    {
        if (EntradaJuego.GetInteractDown() || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMap();
        }
    }

    private void StartGame()
    {
        currentTimer = maxTimeSeconds;
        calibrationCurrentProgress = 0f;
        successfulWelds = 0;
        score = 0;
        probePosition = 0.5f;
        targetZoneCenter = 0.5f;

        instructionText.gameObject.SetActive(false);
        phase1Container.SetActive(true);
        phase2Container.SetActive(false);
        endGameContainer.SetActive(false);

        state = TallerGameState.Phase1Calibration;
    }

    private void UpdateCalibration()
    {
        currentTimer -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTimer <= 0f)
        {
            TriggerGameOver();
            return;
        }

        // Mover zona objetivo oscilante
        targetZoneCenter += targetMoveDirection * targetMoveSpeed * Time.deltaTime;
        if (targetZoneCenter > 0.85f)
        {
            targetZoneCenter = 0.85f;
            targetMoveDirection = -1f;
        }
        else if (targetZoneCenter < 0.15f)
        {
            targetZoneCenter = 0.15f;
            targetMoveDirection = 1f;
        }

        // Control de aguja/sonda por Arduino o Teclado
        float inputX = Input.GetAxisRaw("Horizontal");
        if (ArduinoJoystickInput.Instance != null && ArduinoJoystickInput.Instance.IsInputActive)
        {
            inputX = ArduinoJoystickInput.Instance.Movement.x;
        }

        probePosition = Mathf.Clamp01(probePosition + inputX * 0.75f * Time.deltaTime);

        // Verificar si la aguja está dentro de la zona de tolerancia
        float halfWidth = targetZoneWidth * 0.5f;
        bool inZone = Mathf.Abs(probePosition - targetZoneCenter) <= halfWidth;

        if (inZone)
        {
            calibrationCurrentProgress += Time.deltaTime;
            score += Mathf.RoundToInt(15 * Time.deltaTime);
        }
        else
        {
            calibrationCurrentProgress = Mathf.Max(0f, calibrationCurrentProgress - (Time.deltaTime * 0.4f));
        }

        // Actualizar UI
        float trackWidth = 600f;
        targetZoneRect.anchoredPosition = new Vector2((targetZoneCenter - 0.5f) * trackWidth, 0);
        targetZoneRect.sizeDelta = new Vector2(targetZoneWidth * trackWidth, 40);

        probeIndicatorRect.anchoredPosition = new Vector2((probePosition - 0.5f) * trackWidth, 0);

        var probeImg = probeIndicatorRect.GetComponent<Image>();
        if (probeImg != null)
            probeImg.color = inZone ? new Color(0.2f, 0.9f, 0.3f) : new Color(0.95f, 0.25f, 0.25f);

        float progressFraction = Mathf.Clamp01(calibrationCurrentProgress / calibrationDurationRequired);
        if (calibrationProgressBar != null)
            calibrationProgressBar.fillAmount = progressFraction;

        bool es = LocalizationManager.IsSpanish;
        phaseStatusText.text = es
            ? $"CALIBRACIÓN: {Mathf.RoundToInt(progressFraction * 100)}% (Mantén la aguja dentro de la zona verde)"
            : $"CALIBRATION: {Mathf.RoundToInt(progressFraction * 100)}% (Keep needle inside green zone)";

        if (calibrationCurrentProgress >= calibrationDurationRequired)
        {
            // Completada Fase 1, pasar a Fase 2
            score += 250;
            phase1Container.SetActive(false);
            phase2Container.SetActive(true);
            solderMarkerPosition = 0f;
            state = TallerGameState.Phase2Soldering;
        }
    }

    private void UpdateSoldering()
    {
        currentTimer -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTimer <= 0f)
        {
            TriggerGameOver();
            return;
        }

        // Movimiento de la barra de soldadura
        solderMarkerPosition += solderMarkerSpeed * Time.deltaTime;
        if (solderMarkerPosition > 1f)
            solderMarkerPosition = 0f;

        float trackWidth = 600f;
        solderMarkerRect.anchoredPosition = new Vector2((solderMarkerPosition - 0.5f) * trackWidth, 0);

        bool es = LocalizationManager.IsSpanish;
        phaseStatusText.text = es
            ? $"SOLDADURA DE COMPONENTES ({successfulWelds}/{requiredWelds})"
            : $"COMPONENT SOLDERING ({successfulWelds}/{requiredWelds})";

        // Entrada de botón para soldar
        if (EntradaJuego.GetInteractDown() || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            CheckSolderHit();
        }
    }

    private void CheckSolderHit()
    {
        bool es = LocalizationManager.IsSpanish;
        float center = (solderTargetMin + solderTargetMax) * 0.5f;
        float distToCenter = Mathf.Abs(solderMarkerPosition - center);

        if (solderMarkerPosition >= solderTargetMin && solderMarkerPosition <= solderTargetMax)
        {
            if (distToCenter < 0.04f)
            {
                // Golpe perfecto
                successfulWelds++;
                score += 150;
                ShowFeedback(es ? "¡SOLDADURA PERFECTA! (+150)" : "PERFECT WELD! (+150)", Color.yellow);
            }
            else
            {
                // Golpe bueno
                successfulWelds++;
                score += 80;
                ShowFeedback(es ? "¡BUENA SOLDADURA! (+80)" : "GOOD WELD! (+80)", Color.green);
            }

            solderMarkerPosition = 0f;
            solderMarkerSpeed = Mathf.Min(2.2f, solderMarkerSpeed + 0.15f); // Acelera
        }
        else
        {
            // Fallo
            score = Mathf.Max(0, score - 30);
            ShowFeedback(es ? "PUNTO FRÍO / FALLASTE (-30)" : "COLD JOINT / MISS (-30)", Color.red);
        }

        weldCountText.text = es 
            ? $"Componentes fijados: {successfulWelds} / {requiredWelds}" 
            : $"Components welded: {successfulWelds} / {requiredWelds}";

        if (successfulWelds >= requiredWelds)
        {
            TriggerVictory();
        }
    }

    private void ShowFeedback(string message, Color col)
    {
        if (weldFeedbackText == null) return;
        weldFeedbackText.text = message;
        weldFeedbackText.color = col;
        feedbackTimer = 0.9f;
    }

    private void TriggerVictory()
    {
        PlayerPrefs.SetInt("Taller_Completed", 1);
        PlayerPrefs.Save();
        state = TallerGameState.Victory;
        phase2Container.SetActive(false);
        endGameContainer.SetActive(true);

        int timeBonus = Mathf.RoundToInt(currentTimer * 10);
        score += timeBonus;

        bool es = LocalizationManager.IsSpanish;
        endTitleText.text = es ? "¡CALIBRACIÓN Y REPARACIÓN EXITOSA!" : "CALIBRATION & REPAIR SUCCESSFUL!";
        endTitleText.color = new Color(0.2f, 0.9f, 0.35f);

        string grade = score >= 900 ? "RANGO S" : (score >= 650 ? "RANGO A" : "RANGO B");
        endScoreText.text = es 
            ? $"Puntaje final: {score}  ({grade})\n¡El circuito de ExpoTécnica está operativo!" 
            : $"Final Score: {score}  ({grade})\nThe ExpoTécnica circuit is fully operational!";

        selectedEndOption = 0;
        UpdateEndOptionVisuals();
    }

    private void TriggerGameOver()
    {
        state = TallerGameState.GameOver;
        phase1Container.SetActive(false);
        phase2Container.SetActive(false);
        endGameContainer.SetActive(true);

        bool es = LocalizationManager.IsSpanish;
        endTitleText.text = es ? "¡TIEMPO AGOTADO EN EL TALLER!" : "WORKSHOP TIME UP!";
        endTitleText.color = new Color(0.9f, 0.25f, 0.25f);

        endScoreText.text = es 
            ? $"Puntaje acumulado: {score}\nNo se completaron los ajustes a tiempo." 
            : $"Accumulated Score: {score}\nAdjustments not finished on time.";

        selectedEndOption = 0;
        UpdateEndOptionVisuals();
    }

    private void UpdateEndMenu()
    {
        bool left = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || ArduinoJoystickInput.ConsumeMenuLeft();
        bool right = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || ArduinoJoystickInput.ConsumeMenuRight();

        if (left)
        {
            selectedEndOption = 0;
            UpdateEndOptionVisuals();
        }
        else if (right)
        {
            selectedEndOption = 1;
            UpdateEndOptionVisuals();
        }

        if (EntradaJuego.GetInteractDown() || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedEndOption == 0)
                StartGame();
            else
                ReturnToMap();
        }
    }

    private void UpdateEndOptionVisuals()
    {
        var retryImg = retryButton.GetComponent<Image>();
        var exitImg = exitToMapButton.GetComponent<Image>();

        if (retryImg != null)
            retryImg.color = selectedEndOption == 0 ? new Color(0.2f, 0.6f, 0.95f) : new Color(0.25f, 0.25f, 0.3f);

        if (exitImg != null)
            exitImg.color = selectedEndOption == 1 ? new Color(0.85f, 0.3f, 0.3f) : new Color(0.25f, 0.25f, 0.3f);
    }

    public void ReturnToMap()
    {
        Time.timeScale = 1f;
        MovimientoJugador.canMove = true;
        SceneManager.LoadScene("juego");
    }

    private void ShowInstructions()
    {
        bool es = LocalizationManager.IsSpanish;

        headerTitle.text = es 
            ? "TALLER TÉCNICO: CALIBRACIÓN Y SOLDADURA" 
            : "TECHNICAL WORKSHOP: CALIBRATION & SOLDERING";

        instructionText.text = es
            ? "<b>INSTRUCCIONES PARA EXPOTÉCNICA:</b>\n\n" +
              "1. <b>CALIBRACIÓN:</b> Usa la <b>PALANCA ANALÓGICA</b> (o WASD) para mantener la aguja dentro de la zona verde.\n\n" +
              "2. <b>SOLDADURA:</b> Presiona el <b>BOTÓN FÍSICO</b> (D3 o Espacio/E) justo cuando el contacto esté alineado.\n\n" +
              "<i>¡Juego 100% compatible con el control de arcade Arduino!</i>\n\n" +
              "<b>[ PRESIONA EL BOTÓN FÍSICO (D3 / E) PARA COMENZAR ]</b>"
            : "<b>EXPOTÉCNICA INSTRUCTIONS:</b>\n\n" +
              "1. <b>CALIBRATION:</b> Use the <b>ANALOG STICK</b> (or WASD) to keep the needle inside the green zone.\n\n" +
              "2. <b>SOLDERING:</b> Press the <b>PHYSICAL BUTTON</b> (D3 or Space/E) exactly when aligned.\n\n" +
              "<i>100% playable with the physical Arduino arcade joystick!</i>\n\n" +
              "<b>[ PRESS PHYSICAL BUTTON (D3 / E) TO START ]</b>";

        instructionText.gameObject.SetActive(true);
        phase1Container.SetActive(false);
        phase2Container.SetActive(false);
        endGameContainer.SetActive(false);
    }

    private void UpdateTimerUI()
    {
        bool es = LocalizationManager.IsSpanish;
        timerText.text = es ? $"TIEMPO: {Mathf.CeilToInt(currentTimer)}s" : $"TIME: {Mathf.CeilToInt(currentTimer)}s";
        scoreText.text = es ? $"PUNTAJE: {score}" : $"SCORE: {score}";
    }

    private void BuildWorkshopUI()
    {
        canvasRoot = new GameObject("WorkshopCanvas");
        canvasRoot.transform.SetParent(transform, false);

        var canvas = canvasRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        var scaler = canvasRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasRoot.AddComponent<GraphicRaycaster>();

        // Barra superior de estado
        var headerObj = new GameObject("HeaderPanel", typeof(Image));
        headerObj.transform.SetParent(canvasRoot.transform, false);
        var headerRect = headerObj.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.anchoredPosition = Vector2.zero;
        headerRect.sizeDelta = new Vector2(0, 90);
        headerObj.GetComponent<Image>().color = new Color(0.08f, 0.1f, 0.14f, 0.95f);

        // Título
        var titleObj = new GameObject("HeaderTitle", typeof(TextMeshProUGUI));
        titleObj.transform.SetParent(headerObj.transform, false);
        var titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(40, 0);
        titleRect.sizeDelta = new Vector2(800, 60);
        titleRect.anchorMin = new Vector2(0, 0.5f);
        titleRect.anchorMax = new Vector2(0, 0.5f);
        titleRect.pivot = new Vector2(0, 0.5f);
        headerTitle = titleObj.GetComponent<TextMeshProUGUI>();
        headerTitle.fontSize = 26;
        headerTitle.fontStyle = FontStyles.Bold;
        headerTitle.color = new Color(0.95f, 0.85f, 0.35f);

        // Reloj
        var timeObj = new GameObject("TimerText", typeof(TextMeshProUGUI));
        timeObj.transform.SetParent(headerObj.transform, false);
        var timeRect = timeObj.GetComponent<RectTransform>();
        timeRect.anchoredPosition = new Vector2(-280, 0);
        timeRect.sizeDelta = new Vector2(200, 60);
        timeRect.anchorMin = new Vector2(1, 0.5f);
        timeRect.anchorMax = new Vector2(1, 0.5f);
        timeRect.pivot = new Vector2(1, 0.5f);
        timerText = timeObj.GetComponent<TextMeshProUGUI>();
        timerText.fontSize = 26;
        timerText.fontStyle = FontStyles.Bold;
        timerText.color = Color.white;

        // Puntaje
        var scoreObj = new GameObject("ScoreText", typeof(TextMeshProUGUI));
        scoreObj.transform.SetParent(headerObj.transform, false);
        var scoreRect = scoreObj.GetComponent<RectTransform>();
        scoreRect.anchoredPosition = new Vector2(-40, 0);
        scoreRect.sizeDelta = new Vector2(200, 60);
        scoreRect.anchorMin = new Vector2(1, 0.5f);
        scoreRect.anchorMax = new Vector2(1, 0.5f);
        scoreRect.pivot = new Vector2(1, 0.5f);
        scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
        scoreText.fontSize = 26;
        scoreText.fontStyle = FontStyles.Bold;
        scoreText.color = new Color(0.35f, 0.85f, 1f);

        // Subtítulo / Estado de fase
        var statusObj = new GameObject("PhaseStatusText", typeof(TextMeshProUGUI));
        statusObj.transform.SetParent(canvasRoot.transform, false);
        var statusRect = statusObj.GetComponent<RectTransform>();
        statusRect.anchoredPosition = new Vector2(0, 360);
        statusRect.sizeDelta = new Vector2(1200, 50);
        phaseStatusText = statusObj.GetComponent<TextMeshProUGUI>();
        phaseStatusText.fontSize = 24;
        phaseStatusText.fontStyle = FontStyles.Bold;
        phaseStatusText.alignment = TextAlignmentOptions.Center;
        phaseStatusText.color = Color.white;

        // Panel de Instrucciones
        var instrObj = new GameObject("InstructionText", typeof(TextMeshProUGUI));
        instrObj.transform.SetParent(canvasRoot.transform, false);
        var instrRect = instrObj.GetComponent<RectTransform>();
        instrRect.anchoredPosition = new Vector2(0, 0);
        instrRect.sizeDelta = new Vector2(900, 480);
        instructionText = instrObj.GetComponent<TextMeshProUGUI>();
        instructionText.fontSize = 24;
        instructionText.alignment = TextAlignmentOptions.Center;
        instructionText.color = new Color(0.95f, 0.95f, 0.95f);

        // CONTENEDOR FASE 1: CALIBRACIÓN
        phase1Container = new GameObject("Phase1Container");
        phase1Container.transform.SetParent(canvasRoot.transform, false);

        var p1Card = new GameObject("P1Card", typeof(Image));
        p1Card.transform.SetParent(phase1Container.transform, false);
        var p1CardRect = p1Card.GetComponent<RectTransform>();
        p1CardRect.sizeDelta = new Vector2(800, 340);
        p1Card.GetComponent<Image>().color = new Color(0.12f, 0.14f, 0.18f, 0.95f);

        var p1Title = new GameObject("P1Title", typeof(TextMeshProUGUI));
        p1Title.transform.SetParent(p1Card.transform, false);
        var p1TitleRect = p1Title.GetComponent<RectTransform>();
        p1TitleRect.anchoredPosition = new Vector2(0, 110);
        p1TitleRect.sizeDelta = new Vector2(750, 40);
        var p1TitleTxt = p1Title.GetComponent<TextMeshProUGUI>();
        p1TitleTxt.text = "CALIBRADOR DE FRECUENCIA ANALÓGICO";
        p1TitleTxt.fontSize = 20;
        p1TitleTxt.fontStyle = FontStyles.Bold;
        p1TitleTxt.alignment = TextAlignmentOptions.Center;
        p1TitleTxt.color = new Color(0.9f, 0.9f, 1f);

        // Pista de calibración
        var p1Track = new GameObject("P1Track", typeof(Image));
        p1Track.transform.SetParent(p1Card.transform, false);
        var p1TrackRect = p1Track.GetComponent<RectTransform>();
        p1TrackRect.anchoredPosition = new Vector2(0, 20);
        p1TrackRect.sizeDelta = new Vector2(600, 36);
        p1Track.GetComponent<Image>().color = new Color(0.2f, 0.22f, 0.26f, 1f);

        // Zona objetivo verde
        var p1Target = new GameObject("TargetZone", typeof(Image));
        p1Target.transform.SetParent(p1Track.transform, false);
        targetZoneRect = p1Target.GetComponent<RectTransform>();
        targetZoneRect.sizeDelta = new Vector2(140, 36);
        p1Target.GetComponent<Image>().color = new Color(0.15f, 0.75f, 0.35f, 0.9f);

        // Aguja / Sonda
        var p1Probe = new GameObject("ProbeIndicator", typeof(Image));
        p1Probe.transform.SetParent(p1Track.transform, false);
        probeIndicatorRect = p1Probe.GetComponent<RectTransform>();
        probeIndicatorRect.sizeDelta = new Vector2(16, 52);
        p1Probe.GetComponent<Image>().color = Color.yellow;

        // Barra de progreso de calibración
        var p1ProgBack = new GameObject("ProgBack", typeof(Image));
        p1ProgBack.transform.SetParent(p1Card.transform, false);
        var p1ProgBackRect = p1ProgBack.GetComponent<RectTransform>();
        p1ProgBackRect.anchoredPosition = new Vector2(0, -65);
        p1ProgBackRect.sizeDelta = new Vector2(600, 24);
        p1ProgBack.GetComponent<Image>().color = new Color(0.15f, 0.16f, 0.2f, 1f);

        var p1ProgFill = new GameObject("ProgFill", typeof(Image));
        p1ProgFill.transform.SetParent(p1ProgBack.transform, false);
        var p1ProgFillRect = p1ProgFill.GetComponent<RectTransform>();
        p1ProgFillRect.anchorMin = Vector2.zero;
        p1ProgFillRect.anchorMax = Vector2.one;
        p1ProgFillRect.sizeDelta = Vector2.zero;
        calibrationProgressBar = p1ProgFill.GetComponent<Image>();
        calibrationProgressBar.type = Image.Type.Filled;
        calibrationProgressBar.fillMethod = Image.FillMethod.Horizontal;
        calibrationProgressBar.color = new Color(0.2f, 0.65f, 0.95f, 1f);

        // CONTENEDOR FASE 2: SOLDADURA
        phase2Container = new GameObject("Phase2Container");
        phase2Container.transform.SetParent(canvasRoot.transform, false);

        var p2Card = new GameObject("P2Card", typeof(Image));
        p2Card.transform.SetParent(phase2Container.transform, false);
        var p2CardRect = p2Card.GetComponent<RectTransform>();
        p2CardRect.sizeDelta = new Vector2(800, 340);
        p2Card.GetComponent<Image>().color = new Color(0.14f, 0.12f, 0.16f, 0.95f);

        var p2Title = new GameObject("P2Title", typeof(TextMeshProUGUI));
        p2Title.transform.SetParent(p2Card.transform, false);
        var p2TitleRect = p2Title.GetComponent<RectTransform>();
        p2TitleRect.anchoredPosition = new Vector2(0, 110);
        p2TitleRect.sizeDelta = new Vector2(750, 40);
        var p2TitleTxt = p2Title.GetComponent<TextMeshProUGUI>();
        p2TitleTxt.text = "BANCO DE SOLDADURA DE ALTA PRECISIÓN";
        p2TitleTxt.fontSize = 20;
        p2TitleTxt.fontStyle = FontStyles.Bold;
        p2TitleTxt.alignment = TextAlignmentOptions.Center;
        p2TitleTxt.color = new Color(1f, 0.85f, 0.4f);

        // Pista de soldadura
        var p2Track = new GameObject("SolderTrack", typeof(Image));
        p2Track.transform.SetParent(p2Card.transform, false);
        solderTrackRect = p2Track.GetComponent<RectTransform>();
        solderTrackRect.anchoredPosition = new Vector2(0, 30);
        solderTrackRect.sizeDelta = new Vector2(600, 40);
        p2Track.GetComponent<Image>().color = new Color(0.22f, 0.2f, 0.24f, 1f);

        // Zona verde de impacto
        var p2Target = new GameObject("SolderTarget", typeof(Image));
        p2Target.transform.SetParent(p2Track.transform, false);
        solderTargetZoneRect = p2Target.GetComponent<RectTransform>();
        solderTargetZoneRect.sizeDelta = new Vector2(120, 40);
        p2Target.GetComponent<Image>().color = new Color(0.18f, 0.75f, 0.35f, 0.9f);

        // Marcador móvil
        var p2Marker = new GameObject("SolderMarker", typeof(Image));
        p2Marker.transform.SetParent(p2Track.transform, false);
        solderMarkerRect = p2Marker.GetComponent<RectTransform>();
        solderMarkerRect.sizeDelta = new Vector2(18, 56);
        p2Marker.GetComponent<Image>().color = Color.cyan;

        // Feedback de impacto
        var p2Feedback = new GameObject("SolderFeedback", typeof(TextMeshProUGUI));
        p2Feedback.transform.SetParent(p2Card.transform, false);
        var p2FeedbackRect = p2Feedback.GetComponent<RectTransform>();
        p2FeedbackRect.anchoredPosition = new Vector2(0, -35);
        p2FeedbackRect.sizeDelta = new Vector2(700, 35);
        weldFeedbackText = p2Feedback.GetComponent<TextMeshProUGUI>();
        weldFeedbackText.fontSize = 22;
        weldFeedbackText.fontStyle = FontStyles.Bold;
        weldFeedbackText.alignment = TextAlignmentOptions.Center;

        // Contador de fijados
        var p2Count = new GameObject("SolderCount", typeof(TextMeshProUGUI));
        p2Count.transform.SetParent(p2Card.transform, false);
        var p2CountRect = p2Count.GetComponent<RectTransform>();
        p2CountRect.anchoredPosition = new Vector2(0, -80);
        p2CountRect.sizeDelta = new Vector2(700, 35);
        weldCountText = p2Count.GetComponent<TextMeshProUGUI>();
        weldCountText.fontSize = 20;
        weldCountText.alignment = TextAlignmentOptions.Center;
        weldCountText.color = Color.white;

        // CONTENEDOR FIN DE PARTIDA
        endGameContainer = new GameObject("EndGameContainer");
        endGameContainer.transform.SetParent(canvasRoot.transform, false);

        var endCard = new GameObject("EndCard", typeof(Image));
        endCard.transform.SetParent(endGameContainer.transform, false);
        var endCardRect = endCard.GetComponent<RectTransform>();
        endCardRect.sizeDelta = new Vector2(820, 460);
        endCard.GetComponent<Image>().color = new Color(0.1f, 0.12f, 0.16f, 0.98f);

        var endTitleObj = new GameObject("EndTitle", typeof(TextMeshProUGUI));
        endTitleObj.transform.SetParent(endCard.transform, false);
        var endTitleR = endTitleObj.GetComponent<RectTransform>();
        endTitleR.anchoredPosition = new Vector2(0, 150);
        endTitleR.sizeDelta = new Vector2(780, 60);
        endTitleText = endTitleObj.GetComponent<TextMeshProUGUI>();
        endTitleText.fontSize = 30;
        endTitleText.fontStyle = FontStyles.Bold;
        endTitleText.alignment = TextAlignmentOptions.Center;

        var endScoreObj = new GameObject("EndScore", typeof(TextMeshProUGUI));
        endScoreObj.transform.SetParent(endCard.transform, false);
        var endScoreR = endScoreObj.GetComponent<RectTransform>();
        endScoreR.anchoredPosition = new Vector2(0, 30);
        endScoreR.sizeDelta = new Vector2(780, 100);
        endScoreText = endScoreObj.GetComponent<TextMeshProUGUI>();
        endScoreText.fontSize = 24;
        endScoreText.alignment = TextAlignmentOptions.Center;
        endScoreText.color = Color.white;

        // Botón Reintentar
        var btnRetryObj = new GameObject("BtnRetry", typeof(Image), typeof(Button));
        btnRetryObj.transform.SetParent(endCard.transform, false);
        var btnRetryR = btnRetryObj.GetComponent<RectTransform>();
        btnRetryR.anchoredPosition = new Vector2(-180, -140);
        btnRetryR.sizeDelta = new Vector2(280, 60);
        retryButton = btnRetryObj.GetComponent<Button>();
        retryButton.onClick.AddListener(StartGame);

        var btnRetryTxtObj = new GameObject("TxtRetry", typeof(TextMeshProUGUI));
        btnRetryTxtObj.transform.SetParent(btnRetryObj.transform, false);
        var btnRetryTxtR = btnRetryTxtObj.GetComponent<RectTransform>();
        btnRetryTxtR.sizeDelta = btnRetryR.sizeDelta;
        retryButtonText = btnRetryTxtObj.GetComponent<TextMeshProUGUI>();
        retryButtonText.text = "REINTENTAR";
        retryButtonText.fontSize = 20;
        retryButtonText.fontStyle = FontStyles.Bold;
        retryButtonText.alignment = TextAlignmentOptions.Center;
        retryButtonText.color = Color.white;

        // Botón Volver al Mapa
        var btnExitObj = new GameObject("BtnExit", typeof(Image), typeof(Button));
        btnExitObj.transform.SetParent(endCard.transform, false);
        var btnExitR = btnExitObj.GetComponent<RectTransform>();
        btnExitR.anchoredPosition = new Vector2(180, -140);
        btnExitR.sizeDelta = new Vector2(280, 60);
        exitToMapButton = btnExitObj.GetComponent<Button>();
        exitToMapButton.onClick.AddListener(ReturnToMap);

        var btnExitTxtObj = new GameObject("TxtExit", typeof(TextMeshProUGUI));
        btnExitTxtObj.transform.SetParent(btnExitObj.transform, false);
        var btnExitTxtR = btnExitTxtObj.GetComponent<RectTransform>();
        btnExitTxtR.sizeDelta = btnExitR.sizeDelta;
        exitButtonText = btnExitTxtObj.GetComponent<TextMeshProUGUI>();
        exitButtonText.text = "VOLVER AL MAPA";
        exitButtonText.fontSize = 20;
        exitButtonText.fontStyle = FontStyles.Bold;
        exitButtonText.alignment = TextAlignmentOptions.Center;
        exitButtonText.color = Color.white;
    }
}
