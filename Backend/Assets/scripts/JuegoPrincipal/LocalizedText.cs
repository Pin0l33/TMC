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
        label = GetComponent<TMP_Text>();
    }   

    void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += ApplyLanguage;

        
        if (LocalizationManager.Instance != null)
            ApplyLanguage(LocalizationManager.Instance.CurrentLanguage);
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= ApplyLanguage;
    }

    private void ApplyLanguage(Language language)
    {
        label.text = language == Language.Spanish ? spanishText : englishText;
    }
}