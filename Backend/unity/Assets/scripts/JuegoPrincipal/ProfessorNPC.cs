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

    [Header("Estado posterior al desafío (Opcional)")]
    [SerializeField] private string completionKey = "Taller_Completed";
    [SerializeField, TextArea(3, 5)] private string[] completedLinesEs;
    [SerializeField, TextArea(3, 5)] private string[] completedLinesEn;
    [SerializeField] private string[] completedOptionsEs;
    [SerializeField] private string[] completedOptionsEn;

    public bool IsChallengeCompleted => !string.IsNullOrEmpty(completionKey) && PlayerPrefs.GetInt(completionKey, 0) == 1;

    public static readonly string[] DefaultPreChallengeEs = new string[]
    {
        "¡Hola! Qué alivio verte. Estamos en plena preparación para la ExpoTécnica y el equipo de control del Taller tiene una avería crítica.",
        "El osciloscopio del banco de pruebas perdió la sincronización de frecuencia y varias pistas del circuito están abiertas.",
        "Si no lo calibramos y soldamos a tiempo, no podremos encender el stand de la escuela. ¿Te animás a entrar al Taller a repararlo con la consola arcade?"
    };

    public static readonly string[] DefaultPreChallengeEn = new string[]
    {
        "Hello! What a relief to see you. We are preparing for ExpoTécnica and the Workshop control bench has a critical failure.",
        "The test bench oscilloscope lost frequency synchronization and several circuit traces are open.",
        "If we don't calibrate and solder it on time, we won't be able to power on the school stand. Can you head into the Workshop and fix it using the arcade console?"
    };

    public static readonly string[] DefaultPreOptionsEs = new string[] { "¡Voy al Taller a repararlo!", "Ahora no puedo, profesor." };
    public static readonly string[] DefaultPreOptionsEn = new string[] { "Heading to the Workshop to fix it!", "Not right now, professor." };

    public static readonly string[] DefaultPostChallengeEs = new string[]
    {
        "¡Excelente trabajo en el Taller! El osciloscopio quedó calibrado y las soldaduras están perfectas.",
        "Gracias a vos, el proyecto técnico de la escuela va a brillar en la ExpoTécnica. ¿Querés volver a practicar o seguir recorriendo la escuela?"
    };

    public static readonly string[] DefaultPostChallengeEn = new string[]
    {
        "Outstanding work in the Workshop! The oscilloscope is calibrated and the solder joints are solid.",
        "Thanks to you, our technical project will shine at ExpoTécnica. Would you like to practice again or keep exploring?"
    };

    public static readonly string[] DefaultPostOptionsEs = new string[] { "Volver a practicar en el Taller", "Seguir explorando" };
    public static readonly string[] DefaultPostOptionsEn = new string[] { "Practice again in the Workshop", "Keep exploring" };

    private bool UsesLegacyTestDialogue =>
        dialogueLinesEs != null &&
        dialogueLinesEs.Length == 1 &&
        !string.IsNullOrEmpty(dialogueLinesEs[0]) &&
        dialogueLinesEs[0].Contains("test");

    public string[] DialogueLines
    {
        get
        {
            if (IsChallengeCompleted)
            {
                if (completedLinesEs != null && completedLinesEs.Length > 0)
                {
                    if (LocalizationManager.IsSpanish) return completedLinesEs;
                    return (completedLinesEn != null && completedLinesEn.Length > 0) ? completedLinesEn : completedLinesEs;
                }
                return LocalizationManager.IsSpanish ? DefaultPostChallengeEs : DefaultPostChallengeEn;
            }

            // Si las líneas del Inspector eran solo el test de prueba, usar los diálogos técnicos de TMC
            if (UsesLegacyTestDialogue)
            {
                return LocalizationManager.IsSpanish ? DefaultPreChallengeEs : DefaultPreChallengeEn;
            }

            if (LocalizationManager.IsSpanish) return dialogueLinesEs;
            return (dialogueLinesEn != null && dialogueLinesEn.Length > 0) ? dialogueLinesEn : dialogueLinesEs;
        }
    }

    public string[] Options
    {
        get
        {
            if (IsChallengeCompleted)
            {
                if (completedOptionsEs != null && completedOptionsEs.Length > 0)
                {
                    if (LocalizationManager.IsSpanish) return completedOptionsEs;
                    return (completedOptionsEn != null && completedOptionsEn.Length > 0) ? completedOptionsEn : completedOptionsEs;
                }
                return LocalizationManager.IsSpanish ? DefaultPostOptionsEs : DefaultPostOptionsEn;
            }

            if (UsesLegacyTestDialogue && optionsEs != null && optionsEs.Length == 2 && optionsEs[0].Trim().ToLowerInvariant() == "si")
            {
                return LocalizationManager.IsSpanish ? DefaultPreOptionsEs : DefaultPreOptionsEn;
            }

            if (LocalizationManager.IsSpanish) return optionsEs;
            return (optionsEn != null && optionsEn.Length > 0) ? optionsEn : optionsEs;
        }
    }

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