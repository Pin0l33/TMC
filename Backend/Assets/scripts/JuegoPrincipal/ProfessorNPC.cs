using UnityEngine;

public class ProfessorNPC : MonoBehaviour
{
    [Header("Datos del profesor")]
    [SerializeField] private Sprite boxSprite;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;
    [SerializeField] private string[] options;
    [SerializeField] private string escenaASi;

    [Header("Marca de interaccion")]
    [SerializeField] private GameObject dialogueMark;

    public Sprite BoxSprite => boxSprite;
    public string[] DialogueLines => dialogueLines;
    public string[] Options => options;
    public string EscenaASi => escenaASi;

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
    public void HideMark() => dialogueMark.SetActive(false);
}