using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Video;

/// <summary>Persists night progression and supplies the small result screens used by FNAE.</summary>
public sealed class FNAEProgression : MonoBehaviour
{
    private const string UnlockedKey = "FNAE_MaxUnlockedNight";
    private const string CurrentKey = "FNAE_CurrentNight";
    private static FNAEProgression instance;
    private static bool initialized;

    public static int CurrentNight { get; private set; } = 1;
    public static int MaxUnlockedNight { get; private set; } = 1;
    public static bool IsGameOver { get; private set; }
    private static bool isTestRun;

    private bool resultUiBuilt;
    private bool victoryPresentationFinished;
    private VideoPlayer victoryVideo;
    private Coroutine victoryWatchdog;
    private Coroutine resultFade;
    private CanvasGroup resultCanvasGroup;

    public static void EnsureInitialized()
    {
        if (instance == null)
        {
            var go = new GameObject("FNAE Progression");
            instance = go.AddComponent<FNAEProgression>();
        }

        if (!initialized)
        {
            MaxUnlockedNight = Mathf.Clamp(PlayerPrefs.GetInt(UnlockedKey, 1), 1, 5);
            CurrentNight = Mathf.Clamp(PlayerPrefs.GetInt(CurrentKey, 1), 1, 5);
            initialized = true;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        if (!initialized)
        {
            MaxUnlockedNight = Mathf.Clamp(PlayerPrefs.GetInt(UnlockedKey, 1), 1, 5);
            CurrentNight = Mathf.Clamp(PlayerPrefs.GetInt(CurrentKey, 1), 1, 5);
            initialized = true;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        DetachVictoryVideo();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CancelVictorySequence();
        resultUiBuilt = false;
        IsGameOver = false;
        if (scene.name == "Victory")
            BeginVictoryPresentation();
    }

    public static void StartNewGame()
    {
        EnsureInitialized();
        CurrentNight = 1;
        MaxUnlockedNight = 1;
        IsGameOver = false;
        isTestRun = false;
        SaveProgress();
        PlayerPrefs.SetInt(UnlockedKey, 1);
        PlayerPrefs.Save();
    }

    public static void ContinueUnlockedNight()
    {
        EnsureInitialized();
        CurrentNight = MaxUnlockedNight;
        IsGameOver = false;
        isTestRun = false;
        SaveProgress();
    }

#if UNITY_EDITOR
    /// <summary>Editor-only night selection; it does not unlock or overwrite saved progress.</summary>
    public static void StartTestNight(int night)
    {
        EnsureInitialized();
        CurrentNight = Mathf.Clamp(night, 1, 5);
        IsGameOver = false;
        isTestRun = true;
        SceneManager.LoadScene("Noche1");
    }
#endif

    public static void CompleteCurrentNight()
    {
        EnsureInitialized();
        CurrentNight = Mathf.Clamp(CurrentNight, 1, 5);
        IsGameOver = false;
        if (!isTestRun)
        {
            MaxUnlockedNight = Mathf.Max(MaxUnlockedNight, Mathf.Min(5, CurrentNight + 1));
            SaveProgress();
            PlayerPrefs.SetInt(UnlockedKey, MaxUnlockedNight);
            PlayerPrefs.Save();
        }
    }

    public static float GetEnemyTimeFactor()
    {
        EnsureInitialized();
        // Night 1 keeps authored timings; night 5 moves/attacks at 55% of those intervals.
        return Mathf.Lerp(1f, 0.55f, (Mathf.Clamp(CurrentNight, 1, 5) - 1) / 4f);
    }

    public static void ShowGameOver()
    {
        EnsureInitialized();
        IsGameOver = true;
        if (instance != null)
            instance.BuildGameOverUi();
    }

    private static void SaveProgress()
    {
        PlayerPrefs.SetInt(CurrentKey, CurrentNight);
        PlayerPrefs.Save();
    }

    private void BuildVictoryUi()
    {
        if (resultUiBuilt) return;
        resultUiBuilt = true;

        var canvas = CreateCanvas("FNAE Victory UI");
        resultCanvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
        resultCanvasGroup.alpha = 0f;
        resultCanvasGroup.interactable = false;
        resultCanvasGroup.blocksRaycasts = false;
        CreateResultPanel(canvas.transform, CurrentNight == 5 ? "VICTORIA FINAL\nCOMPLETASTE LAS CINCO NOCHES" : "NOCHE " + CurrentNight + " COMPLETADA");
        if (CurrentNight < 5)
            AddButton(canvas.transform, "CONTINUAR A NOCHE " + (CurrentNight + 1), 0, -55, ContinueToNextNight);
        AddButton(canvas.transform, "MENU PRINCIPAL", 0, CurrentNight < 5 ? -160 : -75, ReturnToMenu);
        resultFade = StartCoroutine(FadeInResults());
    }

    private void BuildGameOverUi()
    {
        if (resultUiBuilt) return;
        resultUiBuilt = true;

        var canvas = CreateCanvas("FNAE Game Over UI");
        resultCanvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
        CreateResultPanel(canvas.transform, "PARTIDA TERMINADA\nNOCHE " + CurrentNight);
        AddButton(canvas.transform, "REINTENTAR NOCHE " + CurrentNight, 0, -55, RetryNight);
        AddButton(canvas.transform, "MENU PRINCIPAL", 0, -160, ReturnToMenu);
        resultCanvasGroup.alpha = 0f;
        resultFade = StartCoroutine(FadeInResults());
    }

    private void BeginVictoryPresentation()
    {
        victoryPresentationFinished = false;
        victoryVideo = null;
        var players = FindObjectsByType<VideoPlayer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].gameObject.scene.name == "Victory")
            {
                victoryVideo = players[i];
                break;
            }
        }

        if (victoryVideo == null || victoryVideo.clip == null)
        {
            victoryWatchdog = StartCoroutine(FinishVictoryAfter(1f));
            return;
        }

        victoryVideo.playOnAwake = false;
        victoryVideo.isLooping = false;
        victoryVideo.prepareCompleted += OnVictoryVideoPrepared;
        victoryVideo.loopPointReached += OnVictoryVideoEnded;
        victoryVideo.errorReceived += OnVictoryVideoError;
        double duration = Mathf.Max(0.1f, (float)victoryVideo.clip.length);
        victoryWatchdog = StartCoroutine(FinishVictoryAfter((float)duration + 2f));
        victoryVideo.Prepare();
    }

    private void OnVictoryVideoPrepared(VideoPlayer source)
    {
        if (!victoryPresentationFinished && source == victoryVideo && source != null)
        {
            if (victoryWatchdog != null)
                StopCoroutine(victoryWatchdog);
            victoryWatchdog = StartCoroutine(FinishVictoryAfter((float)source.clip.length + 1.5f));
            source.Play();
        }
    }

    private void OnVictoryVideoEnded(VideoPlayer source)
    {
        if (source == victoryVideo)
            FinishVictoryPresentation();
    }

    private void OnVictoryVideoError(VideoPlayer source, string message)
    {
        if (source == victoryVideo)
            FinishVictoryPresentation();
    }

    private IEnumerator FinishVictoryAfter(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        FinishVictoryPresentation();
    }

    private void FinishVictoryPresentation()
    {
        if (victoryPresentationFinished || SceneManager.GetActiveScene().name != "Victory")
            return;

        victoryPresentationFinished = true;
        DetachVictoryVideo();
        if (victoryWatchdog != null)
        {
            StopCoroutine(victoryWatchdog);
            victoryWatchdog = null;
        }
        BuildVictoryUi();
    }

    private void DetachVictoryVideo()
    {
        if (victoryVideo != null)
        {
            victoryVideo.prepareCompleted -= OnVictoryVideoPrepared;
            victoryVideo.loopPointReached -= OnVictoryVideoEnded;
            victoryVideo.errorReceived -= OnVictoryVideoError;
            victoryVideo = null;
        }
    }

    private void CancelVictorySequence()
    {
        if (victoryWatchdog != null)
        {
            StopCoroutine(victoryWatchdog);
            victoryWatchdog = null;
        }
        if (resultFade != null)
        {
            StopCoroutine(resultFade);
            resultFade = null;
        }
        DetachVictoryVideo();
        resultCanvasGroup = null;
        victoryPresentationFinished = false;
    }

    private IEnumerator FadeInResults()
    {
        if (resultCanvasGroup == null)
            yield break;

        resultCanvasGroup.interactable = true;
        resultCanvasGroup.blocksRaycasts = true;
        const float fadeDuration = 0.3f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            resultCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        resultCanvasGroup.alpha = 1f;
        resultFade = null;
    }

    private static void CreateResultPanel(Transform parent, string heading)
    {
        var overlay = new GameObject("Fondo de resultados", typeof(RectTransform), typeof(Image));
        overlay.transform.SetParent(parent, false);
        var overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        overlay.GetComponent<Image>().color = new Color(0.015f, 0.025f, 0.035f, 0.94f);

        var panel = new GameObject("Panel de resultados", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(1080f, 520f);
        panelRect.anchoredPosition = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.075f, 0.11f, 0.13f, 0.98f);
        AddLabel(parent, heading, heading.StartsWith("VICTORIA") ? 48 : 54, 0, 112);
    }

    private static Canvas CreateCanvas(string objectName)
    {
        var go = new GameObject(objectName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        if (EventSystem.current == null)
        {
            new GameObject("FNAE Result EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
        return canvas;
    }

    private static void AddLabel(Transform parent, string value, int size, float x, float y)
    {
        var go = new GameObject("Result Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(1400, 180);
        rect.anchoredPosition = new Vector2(x, y);
        var label = go.GetComponent<TextMeshProUGUI>();
        label.text = value;
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = size;
        label.color = Color.white;
        if (TMP_Settings.defaultFontAsset != null)
            label.font = TMP_Settings.defaultFontAsset;
    }

    private static void AddButton(Transform parent, string value, float x, float y, UnityEngine.Events.UnityAction action)
    {
        var go = new GameObject(value, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(660, 78);
        rect.anchoredPosition = new Vector2(x, y);
        var image = go.GetComponent<Image>();
        image.color = new Color(0.12f, 0.19f, 0.21f, 0.98f);
        var button = go.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        AddLabel(go.transform, value, 26, 0, 0);
    }

    private void ContinueToNextNight()
    {
        CurrentNight = Mathf.Clamp(CurrentNight + 1, 1, 5);
        IsGameOver = false;
        if (!isTestRun) SaveProgress();
        SceneManager.LoadScene("TransicionDeNoche");
    }

    private void RetryNight()
    {
        IsGameOver = false;
        resultUiBuilt = false;
        SceneManager.LoadScene("Noche1");
    }

    private void ReturnToMenu()
    {
        IsGameOver = false;
        isTestRun = false;
        resultUiBuilt = false;
        SceneManager.LoadScene("FNAE_Menu");
    }
}
