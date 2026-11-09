using System;
using UnityEngine;

public enum Language { Spanish, English }

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public static event Action<Language> OnLanguageChanged;

    public Language CurrentLanguage { get; private set; }

    private const string PrefsKey = "SelectedLanguage";

    public static Language CurrentSafeLanguage => Instance != null ? Instance.CurrentLanguage : (Language)PlayerPrefs.GetInt(PrefsKey, 0);
    public static bool IsSpanish => CurrentSafeLanguage == Language.Spanish;
    public static bool IsEnglish => CurrentSafeLanguage == Language.English;

    public static void EnsureInitialized()
    {
        if (Instance != null) return;
        var existing = FindFirstObjectByType<LocalizationManager>();
        if (existing != null)
        {
            Instance = existing;
            return;
        }
        var go = new GameObject("LocalizationManager");
        Instance = go.AddComponent<LocalizationManager>();
        DontDestroyOnLoad(go);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }

        int saved = PlayerPrefs.GetInt(PrefsKey, 0); 
        CurrentLanguage = (Language)saved;
    }

    public void SetLanguage(Language language)
    {
        CurrentLanguage = language;

        PlayerPrefs.SetInt(PrefsKey, (int)language);
        PlayerPrefs.Save();

        OnLanguageChanged?.Invoke(language);
    }

    public void SetSpanish()
    {
        SetLanguage(Language.Spanish);
    }

    public void SetEnglish()
    {
        SetLanguage(Language.English);
    }
}