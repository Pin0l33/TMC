using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

public enum BattleState { None, PlayerMenu, FightMinigame, EnemyAttack, Won, Lost, Fled }

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("UI general")]
    [SerializeField] private GameObject battleOverlay;
    [SerializeField] private RectTransform battleBoxRect;
    [SerializeField] private RectTransform soulRect;
    [SerializeField] private Image soulImage;
    [SerializeField] private SoulController soulController;
    [SerializeField] private Image bossPortraitImage;

    [Header("Caja de batalla (cambia de tamaño como en Undertale)")]
    [SerializeField] private RectTransform battleBoxBorderRect;   // opcional: solo si el borde es un Image aparte
    [SerializeField] private float borderThickness = 3f;
    [SerializeField] private Vector2 wideBoxSize = new Vector2(520f, 130f);
    [SerializeField] private Vector2 wideBoxPos = new Vector2(0f, -30f);
    [SerializeField] private Vector2 dodgeBoxSize = new Vector2(220f, 200f);
    [SerializeField] private Vector2 dodgeBoxPos = new Vector2(0f, 10f);
    [SerializeField] private float boxResizeDuration = 0.25f;
    [SerializeField] private TMP_Text boxFlavorText;

    [Header("Diálogo del jefe (globo aparte)")]
    [SerializeField] private GameObject bossSpeechBox;
    [FormerlySerializedAs("battleLogText")]
    [SerializeField] private TMP_Text bossSpeechText;
    [SerializeField] private AudioClip bossVoiceClip;
    [SerializeField] private float bossVoiceMinPitch = 0.75f;
    [SerializeField] private float bossVoiceMaxPitch = 0.95f;

    [Header("Flash de pantalla (cambios de fase)")]
    [SerializeField] private Image screenFlash;

    [Header("Popup de crítico (opcional, no existe en Undertale)")]
    [SerializeField] private GameObject critPopup;
    [SerializeField] private RectTransform critPopupRect;

    [Header("Pulso de HP bajo")]
    [SerializeField] private Image lowHpPulseOverlay;

    [Header("Sonido de texto (narración)")]
    [SerializeField] private AudioClip logBlipClip;
    [SerializeField] private float logBlipMinPitch = 0.9f;
    [SerializeField] private float logBlipMaxPitch = 1.1f;
    [SerializeField] private float logBlipVolume = 0.4f;

    [Header("Menú de turno (sprites)")]
    [SerializeField] private GameObject turnMenu;
    [SerializeField] private Image[] menuButtonImages;          // FIGHT, ACT, ITEM, MERCY
    [SerializeField] private Sprite[] buttonNormalSprites;      // 4 sprites, mismo orden
    [SerializeField] private Sprite[] buttonSelectedSprites;    // 4 sprites, mismo orden
    private int selectedOption;
    private readonly string[] optionLabels = { "FIGHT", "ACT", "ITEM", "MERCY" };

    [Header("Minijuego de ataque (FIGHT, estilo Undertale)")]
    [SerializeField] private GameObject fightBarPanel;           // hijo de BattleBox, ocupa toda la caja ancha
    [SerializeField] private RectTransform fightBarTrack;        // recorrido del cursor (stretch dentro del panel)
    [SerializeField] private RectTransform fightBarMarker;       // el cursor
    [SerializeField] private RectTransform fightBarTargetZone;   // opcional: marca el CENTRO; si queda vacío, el centro es x = 0
    [SerializeField] private float markerSpeed = 600f;           // unidades/seg: una sola pasada de izquierda a derecha
    [SerializeField] private float markerBlinkInterval = 0.08f;
    [SerializeField] private float hitBlinkInterval = 0.04f;
    [SerializeField] private float hitBlinkDuration = 0.4f;
    [SerializeField] private Color markerColorA = Color.white;
    [SerializeField] private Color markerColorB = new Color(0.35f, 0.35f, 0.35f, 1f);
    [SerializeField] private Sprite markerSpriteA;               // opcional: 2 sprites que se alternan (con colores en blanco)
    [SerializeField] private Sprite markerSpriteB;

    [Header("Daño del ataque")]
    [SerializeField] private int minFightDamage = 1;
    [SerializeField] private int maxFightDamage = 12;
    [SerializeField] private float perfectHitWidth = 0.08f;      // fracción del recorrido, alrededor del centro

    [Header("Reacción del jefe al golpe")]
    [SerializeField] private TMP_Text damageNumberText;          // número rojo sobre el jefe
    [SerializeField] private Image enemyMiniHPBack;              // opcional: barra chica, fondo
    [SerializeField] private Image enemyMiniHPFill;              // opcional: barra chica, relleno (Image Type: Filled)
    [SerializeField] private Color damageColor = new Color(1f, 0.15f, 0.15f, 1f);
    [SerializeField] private Color missColor = new Color(0.65f, 0.65f, 0.65f, 1f);
    [SerializeField] private float bossShakeDuration = 0.9f;
    [SerializeField] private float bossShakeMagnitude = 6f;

    [Header("Efecto de corte")]
    [SerializeField] private Image slashEffectImage;
    [SerializeField] private RectTransform slashEffectRect;
    [SerializeField] private float slashMaxWidth = 100f;
    [SerializeField] private float slashAngle = -35f;
    [SerializeField] private Color normalSlashColor = Color.white;
    [SerializeField] private Color perfectSlashColor = new Color(1f, 0.82f, 0.35f);

    [Header("Balas")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("SFX (opcional)")]
    [SerializeField] private ReproducirSFX sfx;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip critClip;
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioClip defeatClip;
    [SerializeField] private AudioClip phaseChangeClip;

    [Header("Música de batalla")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip battleMusic;
    [SerializeField] private AudioClip victoryJingle;

    private int enemyHP;
    private int currentPhase;
    private bool actUsed;
    private bool itemUsed;
    private float actPauseMultiplier = 1f;
    private BossGuardian currentBoss;

    private BattleState state = BattleState.None;
    private Coroutine spawnRoutine;
    private Coroutine fightMarkerRoutine;
    private Coroutine flavorRoutine;
    private Coroutine enemyPhaseRoutine;
    private Coroutine lowHpPulseRoutine;
    private Coroutine hitAnimationRoutine;

    private Image fightMarkerImage;
    private bool resolvingFightHit;

    // estado de la caja (tamaño/posición de reposo + temblor aparte)
    private Vector2 boxSize;
    private Vector2 boxRestPos;
    private Vector2 boxJitter;

    private System.Action<BattleState> onBattleComplete;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        if (battleOverlay != null) battleOverlay.SetActive(false);
        if (critPopup != null) critPopup.SetActive(false);
        if (screenFlash != null) screenFlash.gameObject.SetActive(false);
        if (lowHpPulseOverlay != null) lowHpPulseOverlay.gameObject.SetActive(false);
        if (bossSpeechBox != null) bossSpeechBox.SetActive(false);
        if (damageNumberText != null) damageNumberText.gameObject.SetActive(false);
        if (enemyMiniHPBack != null) enemyMiniHPBack.gameObject.SetActive(false);
        if (enemyMiniHPFill != null) enemyMiniHPFill.gameObject.SetActive(false);

        SnapBox(wideBoxSize, wideBoxPos);
        InitializeSlashEffect();
        InitializeFightBar();
    }

    void Update()
    {
        if (state == BattleState.PlayerMenu)
            HandleMenuInput();
        else if (state == BattleState.FightMinigame && Input.GetKeyDown(KeyCode.E))
            ResolveFightHit();
    }

    // Para acomodar el layout sin correr una pelea (⋮ del componente en el Inspector)
    [ContextMenu("Previsualizar caja ancha")]
    private void PreviewWideBox() { SnapBox(wideBoxSize, wideBoxPos); }

    [ContextMenu("Previsualizar caja de esquive")]
    private void PreviewDodgeBox() { SnapBox(dodgeBoxSize, dodgeBoxPos); }

    // ================= INICIO / FIN =================

    public void StartBattle(BossGuardian boss, string introLine = null, System.Action<BattleState> onComplete = null)
    {
        DialogueManager.Instance.HideContinueIndicator();

        currentBoss = boss;
        onBattleComplete = onComplete;
        battleOverlay.SetActive(true);
        MovimientoJugador.canMove = false;

        PlayerHP.Instance.ResetHP();
        PlayerHP.Instance.OnPlayerDown += HandlePlayerDown;
        PlayerHP.Instance.OnDamaged += HandleDamaged;
        PlayerHP.Instance.OnHPChanged += HandleHPChangedForPulse;

        enemyHP = boss.MaxHP;
        currentPhase = 1;
        actUsed = false;
        itemUsed = false;
        actPauseMultiplier = 1f;
        resolvingFightHit = false;

        if (bossPortraitImage != null)
            bossPortraitImage.enabled = true;   // el Animator ya se encarga de los frames

        SnapBox(wideBoxSize, wideBoxPos);
        HideFlavor();
        if (bossSpeechBox != null) bossSpeechBox.SetActive(false);
        SetSoulVisible(false);
        if (fightBarPanel != null) fightBarPanel.SetActive(false);
        if (turnMenu != null) turnMenu.SetActive(false);

        StartCoroutine(BattleIntro(introLine));
    }

    private IEnumerator BattleIntro(string introLine)
    {
        if (musicSource != null && battleMusic != null)
        {
            musicSource.clip = battleMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        if (!string.IsNullOrEmpty(introLine))
        {
            yield return StartCoroutine(BossSpeak(introLine));
        }
        else
        {
            SetFlavor($"¡{currentBoss.BossName} te desafía!");
            yield return new WaitForSeconds(1.5f);
        }

        EnterPlayerMenu();
    }

    public void StartTestBattle()
    {
        var boss = FindObjectOfType<BossGuardian>();
        if (boss != null) StartBattle(boss);
    }

    private void EndBattle(BattleState result)
    {
        state = result;
        PlayerHP.Instance.OnPlayerDown -= HandlePlayerDown;
        PlayerHP.Instance.OnDamaged -= HandleDamaged;
        PlayerHP.Instance.OnHPChanged -= HandleHPChangedForPulse;

        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        if (fightMarkerRoutine != null) { StopCoroutine(fightMarkerRoutine); fightMarkerRoutine = null; }
        if (hitAnimationRoutine != null) { StopCoroutine(hitAnimationRoutine); hitAnimationRoutine = null; }
        if (enemyPhaseRoutine != null) { StopCoroutine(enemyPhaseRoutine); enemyPhaseRoutine = null; }
        if (lowHpPulseRoutine != null) { StopCoroutine(lowHpPulseRoutine); lowHpPulseRoutine = null; }
        if (lowHpPulseOverlay != null) lowHpPulseOverlay.gameObject.SetActive(false);

        resolvingFightHit = false;
        Time.timeScale = 1f;
        ClearBullets();
        HideFlavor();
        if (bossSpeechBox != null) bossSpeechBox.SetActive(false);
        if (fightBarPanel != null) fightBarPanel.SetActive(false);
        if (damageNumberText != null) damageNumberText.gameObject.SetActive(false);
        if (enemyMiniHPBack != null) enemyMiniHPBack.gameObject.SetActive(false);
        if (enemyMiniHPFill != null) enemyMiniHPFill.gameObject.SetActive(false);
        SetSoulVisible(false);

        if (musicSource != null) musicSource.Stop();

        if (result == BattleState.Won && musicSource != null && victoryJingle != null)
            musicSource.PlayOneShot(victoryJingle);

        battleOverlay.SetActive(false);
        MovimientoJugador.canMove = true;

        onBattleComplete?.Invoke(result);
        onBattleComplete = null;
    }

    // ================= CAJA =================

    private void SnapBox(Vector2 size, Vector2 pos)
    {
        boxSize = size;
        boxRestPos = pos;
        RefreshBox();
    }

    private void RefreshBox()
    {
        battleBoxRect.sizeDelta = boxSize;
        battleBoxRect.anchoredPosition = boxRestPos + boxJitter;

        if (battleBoxBorderRect != null)
        {
            battleBoxBorderRect.sizeDelta = boxSize + Vector2.one * (borderThickness * 2f);
            battleBoxBorderRect.anchoredPosition = boxRestPos + boxJitter;
        }
    }

    private IEnumerator ResizeBox(Vector2 targetSize, Vector2 targetPos)
    {
        Vector2 startSize = boxSize;
        Vector2 startPos = boxRestPos;
        float t = 0f;

        while (t < boxResizeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / boxResizeDuration));
            boxSize = Vector2.Lerp(startSize, targetSize, k);
            boxRestPos = Vector2.Lerp(startPos, targetPos, k);
            RefreshBox();
            yield return null;
        }

        SnapBox(targetSize, targetPos);
    }

    private IEnumerator ShakeBattleBox(float magnitude, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            boxJitter = Random.insideUnitCircle * magnitude;
            RefreshBox();
            yield return null;
        }

        boxJitter = Vector2.zero;
        RefreshBox();
    }

    private void ClearBullets()
    {
        foreach (var b in battleBoxRect.GetComponentsInChildren<Bullet>())
            Destroy(b.gameObject);
    }

    private void SetSoulVisible(bool visible)
    {
        if (soulImage != null) soulImage.enabled = visible;
        if (soulController != null) soulController.enabled = false;
    }

    // ================= TEXTO =================

    // Narración dentro de la caja, con "* " como en Undertale
    private void SetFlavor(string text)
    {
        if (boxFlavorText == null) return;
        boxFlavorText.gameObject.SetActive(true);
        if (flavorRoutine != null) StopCoroutine(flavorRoutine);
        flavorRoutine = StartCoroutine(TypeText(boxFlavorText, "* " + text, logBlipClip, logBlipMinPitch, logBlipMaxPitch, logBlipVolume));
    }

    private void HideFlavor()
    {
        if (flavorRoutine != null) { StopCoroutine(flavorRoutine); flavorRoutine = null; }
        if (boxFlavorText != null) boxFlavorText.gameObject.SetActive(false);
    }

    // El jefe habla en su globo, con su propia voz
    private IEnumerator BossSpeak(string text)
    {
        if (bossSpeechText == null) yield break;
        if (bossSpeechBox != null) bossSpeechBox.SetActive(true);

        AudioClip voice = bossVoiceClip != null ? bossVoiceClip : logBlipClip;
        yield return StartCoroutine(TypeText(bossSpeechText, text, voice, bossVoiceMinPitch, bossVoiceMaxPitch, logBlipVolume));
        yield return new WaitForSeconds(1.2f);

        if (bossSpeechBox != null) bossSpeechBox.SetActive(false);
    }

    private IEnumerator TypeText(TMP_Text target, string text, AudioClip blip, float minPitch, float maxPitch, float volume)
    {
        target.text = text;
        target.maxVisibleCharacters = 0;

        for (int i = 0; i < text.Length; i++)
        {
            target.maxVisibleCharacters = i + 1;

            if (!char.IsWhiteSpace(text[i]) && blip != null && sfx != null && i % 2 == 0)
                sfx.PlayInterrupting(blip, minPitch, maxPitch, volume);

            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    // ================= EFECTOS =================

    private void HandlePlayerDown()
    {
        PlaySfx(defeatClip);
        SetFlavor("Te quedaste sin fuerzas...");
        EndBattle(BattleState.Lost);
    }

    private void HandleDamaged(int amount)
    {
        PlaySfx(hitClip);
        StartCoroutine(ShakeBattleBox(6f, 0.2f));
        StartCoroutine(FlashSoul());
    }

    private void HandleHPChangedForPulse(int current, int max)
    {
        bool low = current > 0 && current <= max * 0.3f;

        if (low && lowHpPulseRoutine == null)
            lowHpPulseRoutine = StartCoroutine(LowHpPulseLoop());
        else if (!low && lowHpPulseRoutine != null)
        {
            StopCoroutine(lowHpPulseRoutine);
            lowHpPulseRoutine = null;
            if (lowHpPulseOverlay != null) lowHpPulseOverlay.gameObject.SetActive(false);
        }
    }

    private IEnumerator LowHpPulseLoop()
    {
        if (lowHpPulseOverlay == null) yield break;
        lowHpPulseOverlay.gameObject.SetActive(true);

        while (true)
        {
            float a = Mathf.PingPong(Time.unscaledTime * 1.5f, 0.35f);
            Color c = lowHpPulseOverlay.color;
            c.a = a;
            lowHpPulseOverlay.color = c;
            yield return null;
        }
    }

    private IEnumerator FlashSoul()
    {
        if (soulImage == null) yield break;
        Color original = soulImage.color;
        soulImage.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        soulImage.color = original;
    }

    private IEnumerator ScreenFlashOnce()
    {
        if (screenFlash == null) yield break;
        screenFlash.gameObject.SetActive(true);
        Color c = screenFlash.color;
        c.a = 1f;
        screenFlash.color = c;

        float t = 0f;
        float duration = 0.25f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / duration);
            screenFlash.color = c;
            yield return null;
        }
        screenFlash.gameObject.SetActive(false);
    }

    private IEnumerator PunchScale(RectTransform rt, float fromScale, float toScale, float duration)
    {
        if (rt == null) yield break;
        float t = 0f;
        rt.localScale = Vector3.one * fromScale;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float s = Mathf.Lerp(fromScale, toScale, t / duration);
            rt.localScale = Vector3.one * s;
            yield return null;
        }
        rt.localScale = Vector3.one * toScale;
    }

    // ================= MENÚ =================

    private void EnterPlayerMenu()
    {
        state = BattleState.PlayerMenu;
        turnMenu.SetActive(true);
        if (fightBarPanel != null) fightBarPanel.SetActive(false);
        SetSoulVisible(false);
        selectedOption = 0;
        UpdateMenuUI(selectedOption);
        SetFlavor($"{currentBoss.BossName} bloquea el camino.");
    }

    private void HandleMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedOption = Mathf.Max(0, selectedOption - 1);
            UpdateMenuUI(selectedOption);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedOption = Mathf.Min(optionLabels.Length - 1, selectedOption + 1);
            UpdateMenuUI(selectedOption);
        }
        if (Input.GetKeyDown(KeyCode.E))
            SelectMenuOption();
    }

    // highlighted = -1 deja los 4 botones en su sprite normal (turno del enemigo)
    private void UpdateMenuUI(int highlighted)
    {
        for (int i = 0; i < menuButtonImages.Length; i++)
        {
            menuButtonImages[i].sprite = (i == highlighted)
                ? buttonSelectedSprites[i]
                : buttonNormalSprites[i];
        }
    }

    private void SelectMenuOption()
    {
        switch (selectedOption)
        {
            case 0: StartFightMinigame(); break;
            case 1: DoAct(); break;
            case 2: DoItem(); break;
            case 3: DoMercy(); break;
        }
    }

    // ================= FIGHT =================

    private void InitializeFightBar()
    {
        fightMarkerImage = fightBarMarker != null ? fightBarMarker.GetComponent<Image>() : null;
        ApplyMarkerLook(false);
    }

    // El cursor alterna entre dos aspectos (como el parpadeo blanco/negro de Undertale)
    private void ApplyMarkerLook(bool alt)
    {
        if (fightMarkerImage == null) return;
        if (markerSpriteA != null && markerSpriteB != null)
            fightMarkerImage.sprite = alt ? markerSpriteB : markerSpriteA;
        fightMarkerImage.color = alt ? markerColorB : markerColorA;
    }

    private float GetFightCenterX()
    {
        return fightBarTargetZone != null ? fightBarTargetZone.anchoredPosition.x : 0f;
    }

    private float GetFightHalfTravel()
    {
        return fightBarTrack.rect.width / 2f - fightBarMarker.rect.width / 2f;
    }

    private Vector3 GetBossWorldPos()
    {
        return bossPortraitImage != null
            ? bossPortraitImage.rectTransform.position
            : battleBoxRect.position;
    }

    private void StartFightMinigame()
    {
        if (fightBarPanel == null || fightBarTrack == null || fightBarMarker == null)
        {
            Debug.LogError("BattleManager: Asigná FightBarPanel, FightBarTrack y FightBarMarker en el Inspector.", this);
            return;
        }

        if (slashEffectImage != null)
            slashEffectImage.gameObject.SetActive(false);

        state = BattleState.FightMinigame;
        resolvingFightHit = false;

        HideFlavor();
        fightBarPanel.SetActive(true);
        fightBarMarker.localScale = Vector3.one;
        ApplyMarkerLook(false);

        fightMarkerRoutine = StartCoroutine(MoveFightMarker());
    }

    // Una sola pasada de izquierda a derecha, parpadeando. Si llega al final: MISS.
    private IEnumerator MoveFightMarker()
    {
        float halfTravel = GetFightHalfTravel();

        Vector2 pos = fightBarMarker.anchoredPosition;
        pos.x = -halfTravel;
        fightBarMarker.anchoredPosition = pos;

        float blinkTimer = 0f;
        bool alt = false;

        while (pos.x < halfTravel)
        {
            pos.x += markerSpeed * Time.deltaTime;
            fightBarMarker.anchoredPosition = pos;

            blinkTimer += Time.unscaledDeltaTime;
            if (blinkTimer >= markerBlinkInterval)
            {
                blinkTimer = 0f;
                alt = !alt;
                ApplyMarkerLook(alt);
            }

            yield return null;
        }

        fightMarkerRoutine = null;
        ResolveFightMiss();
    }

    private void ResolveFightHit()
    {
        if (resolvingFightHit) return;
        resolvingFightHit = true;
        state = BattleState.None;

        if (fightMarkerRoutine != null)
        {
            StopCoroutine(fightMarkerRoutine);
            fightMarkerRoutine = null;
        }

        float halfTravel = GetFightHalfTravel();
        float centerX = GetFightCenterX();
        float maxDistance = Mathf.Max(
            Mathf.Abs(-halfTravel - centerX),
            Mathf.Abs(halfTravel - centerX),
            0.001f);

        float distance = Mathf.Abs(fightBarMarker.anchoredPosition.x - centerX);
        float precision = Mathf.Clamp01(1f - distance / maxDistance);

        bool isCrit = distance / maxDistance <= perfectHitWidth;
        int damage = isCrit
            ? maxFightDamage
            : Mathf.RoundToInt(Mathf.Lerp(minFightDamage, maxFightDamage, precision));

        hitAnimationRoutine = StartCoroutine(FightHitSequence(damage, isCrit, false));
    }

    private void ResolveFightMiss()
    {
        if (resolvingFightHit) return;
        resolvingFightHit = true;
        state = BattleState.None;

        hitAnimationRoutine = StartCoroutine(FightHitSequence(0, false, true));
    }

    private IEnumerator FightHitSequence(int damage, bool isCrit, bool miss)
    {
        // 1) el cursor parpadea rápido donde lo frenaste
        if (!miss)
        {
            float t = 0f;
            float timer = 0f;
            bool alt = false;

            while (t < hitBlinkDuration)
            {
                t += Time.unscaledDeltaTime;
                timer += Time.unscaledDeltaTime;

                if (timer >= hitBlinkInterval)
                {
                    timer = 0f;
                    alt = !alt;
                    ApplyMarkerLook(alt);
                }

                yield return null;
            }
        }

        fightBarPanel.SetActive(false);
        ApplyMarkerLook(false);

        // 2) reacción del jefe
        if (miss)
        {
            yield return StartCoroutine(ShowDamageNumber("MISS", missColor, enemyHP, false));
        }
        else
        {
            PlaySfx(isCrit ? critClip : hitClip);
            if (isCrit) StartCoroutine(ShowCritPopup());

            int hpBefore = enemyHP;
            enemyHP = Mathf.Max(0, enemyHP - damage);

            // el corte aparece sobre el enemigo
            yield return StartCoroutine(PlaySlashEffect(GetBossWorldPos(), isCrit));

            StartCoroutine(ShakeBoss());
            yield return StartCoroutine(ShowDamageNumber(damage.ToString(), damageColor, hpBefore, true));
        }

        hitAnimationRoutine = null;
        resolvingFightHit = false;

        if (enemyHP <= 0)
        {
            PlaySfx(victoryClip);
            StartCoroutine(ShakeBattleBox(10f, 0.4f));
            StartCoroutine(VictorySequence());
        }
        else
        {
            StartEnemyPhase(0.2f);
        }
    }

    // Número de daño sobre el jefe (y, opcional, su barra de vida un instante)
    private IEnumerator ShowDamageNumber(string text, Color color, int hpBefore, bool showBar)
    {
        if (damageNumberText != null)
        {
            damageNumberText.gameObject.SetActive(true);
            damageNumberText.text = text;
            damageNumberText.color = color;
            StartCoroutine(PunchScale(damageNumberText.rectTransform, 1.5f, 1f, 0.15f));
        }

        bool barOn = showBar && enemyMiniHPBack != null && enemyMiniHPFill != null && currentBoss != null;
        if (barOn)
        {
            enemyMiniHPBack.gameObject.SetActive(true);
            enemyMiniHPFill.gameObject.SetActive(true);

            float from = (float)hpBefore / currentBoss.MaxHP;
            float to = (float)enemyHP / currentBoss.MaxHP;
            enemyMiniHPFill.fillAmount = from;

            float t = 0f;
            float duration = 0.4f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                enemyMiniHPFill.fillAmount = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
            enemyMiniHPFill.fillAmount = to;
        }

        yield return new WaitForSecondsRealtime(0.7f);

        if (damageNumberText != null) damageNumberText.gameObject.SetActive(false);
        if (barOn)
        {
            enemyMiniHPBack.gameObject.SetActive(false);
            enemyMiniHPFill.gameObject.SetActive(false);
        }
    }

    // El jefe tiembla de costado, con la sacudida apagándose
    private IEnumerator ShakeBoss()
    {
        if (bossPortraitImage == null) yield break;

        var rt = bossPortraitImage.rectTransform;
        Vector2 original = rt.anchoredPosition;
        float t = 0f;

        while (t < bossShakeDuration)
        {
            t += Time.unscaledDeltaTime;
            float decay = 1f - Mathf.Clamp01(t / bossShakeDuration);
            rt.anchoredPosition = original + new Vector2(Mathf.Sin(t * 45f) * bossShakeMagnitude * decay, 0f);
            yield return null;
        }

        rt.anchoredPosition = original;
    }

    private IEnumerator PlaySlashEffect(Vector3 worldPos, bool isCrit)
    {
        if (slashEffectImage == null || slashEffectRect == null)
            yield break;

        // Se respeta el tamaño, la rotación y la escala base del Inspector.
        Vector3 baseScale = slashEffectRect.localScale;
        Color baseColor = isCrit ? perfectSlashColor : normalSlashColor;
        float duration = isCrit ? 0.22f : 0.16f;

        slashEffectRect.position = worldPos;
        slashEffectImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
        slashEffectImage.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pulse = 1f + Mathf.Sin(t * Mathf.PI) * (isCrit ? 0.35f : 0.15f);
            slashEffectRect.localScale = new Vector3(baseScale.x * pulse, baseScale.y * pulse, baseScale.z);
            slashEffectImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f - t);
            yield return null;
        }

        slashEffectRect.localScale = baseScale;
        slashEffectImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        slashEffectImage.gameObject.SetActive(false);
    }

    private void InitializeSlashEffect()
    {
        if (slashEffectImage == null || slashEffectRect == null)
            return;

        slashEffectImage.raycastTarget = false;
        Color color = slashEffectImage.color;
        color.a = 0f;
        slashEffectImage.color = color;
        slashEffectImage.gameObject.SetActive(false);
    }

    private IEnumerator VictorySequence()
    {
        yield return new WaitForSeconds(0.6f);

        HideFlavor();
        string line = currentBoss.DefeatLine;
        if (!string.IsNullOrEmpty(line))
        {
            yield return StartCoroutine(BossSpeak(line));
        }
        else
        {
            SetFlavor($"¡{currentBoss.BossName} fue derrotado!");
            yield return new WaitForSeconds(1.2f);
        }

        EndBattle(BattleState.Won);
    }

    private IEnumerator ShowCritPopup()
    {
        if (critPopup == null) yield break;
        critPopup.SetActive(true);
        yield return StartCoroutine(PunchScale(critPopupRect, 0.3f, 1.2f, 0.15f));
        yield return new WaitForSeconds(0.4f);
        critPopup.SetActive(false);
    }

    // ================= ACT / ITEM / MERCY =================

    private void DoAct()
    {
        if (actUsed)
        {
            SetFlavor("Ya le pediste una pausa, no va a caer de nuevo.");
        }
        else
        {
            actUsed = true;
            actPauseMultiplier = 2f;
            SetFlavor($"Le pedís una pausa a {currentBoss.BossName}. Parece más tranquilo esta vez...");
        }
        StartEnemyPhase();
    }

    private void DoItem()
    {
        if (itemUsed)
        {
            SetFlavor("Ya usaste tu objeto.");
        }
        else
        {
            itemUsed = true;
            PlayerHP.Instance.Heal(currentBoss.ItemHealAmount);
            SetFlavor($"Usás {currentBoss.ItemName}. Recuperás {currentBoss.ItemHealAmount} HP.");
        }
        StartEnemyPhase();
    }

    private void DoMercy()
    {
        StartCoroutine(FleeSequence());
    }

    private IEnumerator FleeSequence()
    {
        state = BattleState.None;
        SetFlavor("Decidís huir...");
        yield return new WaitForSeconds(1.2f);
        EndBattle(BattleState.Fled);
    }

    // ================= TURNO DEL ENEMIGO =================

    // readDelay = tiempo para leer el texto de tu acción antes de que el jefe hable
    private void StartEnemyPhase(float readDelay = 1.2f)
    {
        UpdateMenuUI(-1);
        if (enemyPhaseRoutine != null) StopCoroutine(enemyPhaseRoutine);
        enemyPhaseRoutine = StartCoroutine(EnemyAttackPhase(readDelay));
    }

    private IEnumerator EnemyAttackPhase(float readDelay)
    {
        state = BattleState.EnemyAttack;
        SetSoulVisible(false);

        // 1) un momento para leer el resultado de tu acción (texto dentro de la caja)
        if (readDelay > 0f)
            yield return new WaitForSeconds(readDelay);

        // 2) ¿cambia de fase?
        int phase = 1;
        if (enemyHP <= currentBoss.MaxHP * 0.25f) phase = 3;
        else if (enemyHP <= currentBoss.MaxHP * 0.5f) phase = 2;

        string speech;
        if (phase > currentPhase)
        {
            currentPhase = phase;
            PlaySfx(phaseChangeClip);
            speech = phase == 3 ? currentBoss.EnrageLine : currentBoss.LowHpLine;

            yield return StartCoroutine(ScreenFlashOnce());
            yield return StartCoroutine(ShakeBattleBox(12f, 0.3f));

            Time.timeScale = 0.3f;
            yield return new WaitForSecondsRealtime(0.5f);
            Time.timeScale = 1f;
        }
        else
        {
            speech = currentBoss.GetTauntLine();
        }

        // 3) el jefe habla en su globo (la caja queda vacía)
        HideFlavor();
        if (!string.IsNullOrEmpty(speech))
            yield return StartCoroutine(BossSpeak(speech));

        // 4) la caja se achica y arranca el ataque
        yield return StartCoroutine(ResizeBox(dodgeBoxSize, dodgeBoxPos));

        soulRect.anchoredPosition = Vector2.zero;
        SetSoulVisible(true);
        soulController.enabled = true;

        float multiplier = actPauseMultiplier;
        actPauseMultiplier = 1f;
        spawnRoutine = StartCoroutine(SpawnBulletsPhase(phase, multiplier));

        yield return new WaitForSeconds(currentBoss.PhaseDuration(Mathf.Min(phase, 2)));

        // 5) fin del ataque: se limpian las balas y la caja vuelve a ser ancha
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        ClearBullets();
        SetSoulVisible(false);

        yield return StartCoroutine(ResizeBox(wideBoxSize, wideBoxPos));

        if (state == BattleState.EnemyAttack)
            EnterPlayerMenu();
    }

    private IEnumerator SpawnBulletsPhase(int phase, float intervalMultiplier)
    {
        int configPhase = Mathf.Min(phase, 2);
        float interval = currentBoss.PhaseInterval(configPhase) * intervalMultiplier;
        float speed = currentBoss.PhaseBulletSpeed(configPhase);

        if (phase == 3) { interval *= 0.6f; speed *= 1.2f; }

        while (true)
        {
            yield return new WaitForSeconds(interval);

            float halfWidth = battleBoxRect.rect.width / 2f;
            float halfHeight = battleBoxRect.rect.height / 2f;

            if (phase == 1)
            {
                float spawnX = Random.Range(-halfWidth, halfWidth);
                SpawnOneBullet(new Vector2(spawnX, halfHeight), Vector2.down, speed);
            }
            else
            {
                float spawnY = Random.Range(-halfHeight * 0.7f, halfHeight * 0.7f);
                SpawnOneBullet(new Vector2(-halfWidth, spawnY), Vector2.right, speed);

                if (phase == 3)
                {
                    float spawnY2 = Random.Range(-halfHeight * 0.7f, halfHeight * 0.7f);
                    SpawnOneBullet(new Vector2(halfWidth, spawnY2), Vector2.left, speed);
                }
            }
        }
    }

    private void SpawnOneBullet(Vector2 spawnPos, Vector2 dir, float speed)
    {
        var bulletObj = Instantiate(bulletPrefab, battleBoxRect);
        bulletObj.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        bulletObj.GetComponent<Bullet>().Init(dir, soulRect, battleBoxRect, speed);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfx != null && clip != null)
            sfx.PlaySFX(clip);
    }
}