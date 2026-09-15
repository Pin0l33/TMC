using UnityEngine;

public class ProfessorNPC : MonoBehaviour
{
    [Header("Datos del profesor")]
    [SerializeField] private Sprite boxSprite;

    [Header("Diálogo (Español)")]
    [SerializeField, TextArea(4, 6)] private string[] dialogueLinesEs;

    [Header("Dialogue (English)")]
    [SerializeField, TextArea(4, 6)] private string[] dialogueLinesEn;

    [Header("Opciones (Español)")]
    [SerializeField] private string[] optionsEs;

    [Header("Options (English)")]
    [SerializeField] private string[] optionsEn;

    [SerializeField] private string escenaASi;

    [Header("Marca de interacción")]
    [SerializeField] private GameObject dialogueMark;

    public Sprite BoxSprite => boxSprite;

    public string[] DialogueLines =>
        LocalizationManager.Instance.CurrentLanguage == Language.Spanish ? dialogueLinesEs : dialogueLinesEn;

    public string[] Options =>
        LocalizationManager.Instance.CurrentLanguage == Language.Spanish ? optionsEs : optionsEn;

    public string EscenaASi => escenaASi;

    public void HideMark() => dialogueMark.SetActive(false);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
        {
            dialogueMark.SetActive(true);
            DialogueManager.Instance.SetNearbyNPC(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
        {
            dialogueMark.SetActive(false);
            DialogueManager.Instance.ClearNearbyNPC(this);
        }
    } 
}