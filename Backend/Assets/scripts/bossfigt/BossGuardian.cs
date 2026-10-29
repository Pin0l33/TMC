using UnityEngine;

public class BossGuardian : MonoBehaviour
{
    [Header("Fase 3 - Enrage (HP <= 25%)")]
[SerializeField, TextArea(1, 2)] private string enrageLineEs;
[SerializeField, TextArea(1, 2)] private string enrageLineEn;

[Header("Recompensa")]
[SerializeField] private int coinReward = 20;
public int CoinReward => coinReward;

public string EnrageLine => LocalizationManager.Instance.CurrentLanguage == Language.Spanish ? enrageLineEs : enrageLineEn;

    [Header("Identidad")]
    [SerializeField] private string bossName;
    [SerializeField] private Sprite bossPortrait;

    [Header("Diálogo (Español)")]
    [SerializeField, TextArea(1, 2)] private string[] tauntLinesEs;
    [SerializeField, TextArea(1, 2)] private string lowHpLineEs;
    [SerializeField, TextArea(1, 2)] private string defeatLineEs;

    [Header("Dialogue (English)")]
    [SerializeField, TextArea(1, 2)] private string[] tauntLinesEn;
    [SerializeField, TextArea(1, 2)] private string lowHpLineEn;
    [SerializeField, TextArea(1, 2)] private string defeatLineEn;

    [Header("Ítem especial de esta batalla")]
    [SerializeField] private string itemNameEs = "Tuerca de la suerte";
    [SerializeField] private string itemNameEn = "Lucky nut";
    [SerializeField] private int itemHealAmount = 6;

    [Header("Config de batalla")]
    [SerializeField] private int maxHP = 2000;

    [Header("Fase 1 (HP > 50%)")]
    [SerializeField] private float phase1Interval = 0.6f;
    [SerializeField] private float phase1Duration = 4f;
    [SerializeField] private float phase1BulletSpeed = 180f;

    [Header("Fase 2 (HP <= 50%, más agresivo)")]
    [SerializeField] private float phase2Interval = 0.35f;
    [SerializeField] private float phase2Duration = 5f;
    [SerializeField] private float phase2BulletSpeed = 260f;

    public string BossName => string.IsNullOrEmpty(bossName) ? gameObject.name : bossName;
    public Sprite BossPortrait => bossPortrait;
    public int MaxHP => maxHP;

    public float PhaseInterval(int phase) => phase == 1 ? phase1Interval : phase2Interval;
    public float PhaseDuration(int phase) => phase == 1 ? phase1Duration : phase2Duration;
    public float PhaseBulletSpeed(int phase) => phase == 1 ? phase1BulletSpeed : phase2BulletSpeed;

    public string GetTauntLine()
    {
        bool es = LocalizationManager.Instance.CurrentLanguage == Language.Spanish;
        var lines = es ? tauntLinesEs : tauntLinesEn;
        if (lines == null || lines.Length == 0) return "";
        return lines[Random.Range(0, lines.Length)];
    }

    public string LowHpLine => LocalizationManager.Instance.CurrentLanguage == Language.Spanish ? lowHpLineEs : lowHpLineEn;
    public string DefeatLine => LocalizationManager.Instance.CurrentLanguage == Language.Spanish ? defeatLineEs : defeatLineEn;

    public string ItemName => LocalizationManager.Instance.CurrentLanguage == Language.Spanish ? itemNameEs : itemNameEn;
    public int ItemHealAmount => itemHealAmount;
}