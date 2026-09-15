using System;
using UnityEngine;

public enum Language { Spanish, English }

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public static event Action<Language> OnLanguageChanged;

    public Language CurrentLanguage { get; private set; }

    private const string PrefsKey = "SelectedLanguage";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

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
}}