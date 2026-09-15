using UnityEngine;
using TMPro;

public class LanguageSelectorUI : MonoBehaviour
{
    [SerializeField] private TMP_Text spanishText;
    [SerializeField] private TMP_Text englishText;

    [SerializeField] private Color activeColor = new Color(1f, 0.302f, 0.427f);   // #ff4d6d
    [SerializeField] private Color inactiveColor = Color.white;

    void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateHighlight;

        if (LocalizationManager.Instance != null)
            UpdateHighlight(LocalizationManager.Instance.CurrentLanguage);
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateHighlight;
    }

    private void UpdateHighlight(Language language)
    {
        spanishText.color = language == Language.Spanish ? activeColor : inactiveColor;
        englishText.color = language == Language.English ? activeColor : inactiveColor;
    }
}