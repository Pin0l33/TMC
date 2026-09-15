using UnityEngine;

public class LanguageButtonsProxy : MonoBehaviour
{
    public void SetSpanish() => LocalizationManager.Instance.SetSpanish();
    public void SetEnglish() => LocalizationManager.Instance.SetEnglish();
}