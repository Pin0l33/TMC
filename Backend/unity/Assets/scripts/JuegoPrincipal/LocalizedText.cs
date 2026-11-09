using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [Header("Traducciones")]
    [TextArea(1, 3)] [SerializeField] private string spanishText;
    [TextArea(1, 3)] [SerializeField] private string englishText;

    private TMP_Text label;

    void Awake()
    {
        if (label == null) label = GetComponent<TMP_Text>();
    }   

    void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += ApplyLanguage;
        ApplyLanguage(LocalizationManager.CurrentSafeLanguage);
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= ApplyLanguage;
    }

    private void ApplyLanguage(Language language)
    {
        if (label == null) label = GetComponent<TMP_Text>();
        if (label == null) return;
        string text = language == Language.Spanish ? spanishText : englishText;
        if (string.IsNullOrEmpty(text)) text = spanishText;
        label.text = text;
    }
}