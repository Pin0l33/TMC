using UnityEngine;

public class LanguageButtonsProxy : MonoBehaviour
{
    public void SetSpanish()
    {
        LocalizationManager.EnsureInitialized();
        LocalizationManager.Instance.SetSpanish();
    }

    public void SetEnglish()
    {
        LocalizationManager.EnsureInitialized();
        LocalizationManager.Instance.SetEnglish();
    }
}