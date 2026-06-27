using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogo : MonoBehaviour
{
    private bool isPlayerInRange;

    [Header("UI")]
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Opciones")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private TMP_Text[] optionsText;
    [SerializeField] private string[] options;
    [SerializeField] private string escenaASi;

    [Header("Dialogo")]
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;

    private float typingTime = 0.05f;
    private bool didDialogueStart;
    private int lineIndex;
    private bool isTyping;

    private bool isChoosing;
    private int currentOption;

    private Coroutine typingCoroutine;

    void Update()
    {
        
        if (isChoosing)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentOption = Mathf.Max(0, currentOption - 1);
                UpdateOptionUI();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentOption = Mathf.Min(options.Length - 1, currentOption + 1);
                UpdateOptionUI();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                SelectOption();
            }

            return;
        }

        
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!didDialogueStart)
            {
                StartDialogue();
            }
            else if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueLines[lineIndex];
                isTyping = false;
            }
            else
            {
                NextDialogueLine();
            }
        }
    }

    private void StartDialogue()
    {
        if (dialogueLines.Length == 0) return;

        didDialogueStart = true;
        MovimientoJugador.canMove = false; 

        dialoguePanel.SetActive(true);
        dialogueMark.SetActive(false);
        lineIndex = 0;

        typingCoroutine = StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;

        if (lineIndex < dialogueLines.Length)
        {
            typingCoroutine = StartCoroutine(ShowLine());
        }
        else
        {
            ShowOptions();
        }
    }

    private IEnumerator ShowLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }

        isTyping = false;
    }

    

    private void ShowOptions()
    {
        if (options.Length == 0)
        {
            EndDialogue();
            return;
        }

        isChoosing = true;
        MovimientoJugador.canMove = false; 

        dialoguePanel.SetActive(false); 
        optionsPanel.SetActive(true);

        currentOption = 0;

        for (int i = 0; i < optionsText.Length; i++)
        {
            if (i < options.Length)
                optionsText[i].text = options[i];
            else
                optionsText[i].text = "";
        }

        UpdateOptionUI();
    }

    private void UpdateOptionUI()
    {
        for (int i = 0; i < optionsText.Length; i++)
        {
            if (i < options.Length)
            {
                optionsText[i].text = (i == currentOption ? "> " : "  ") + options[i];
            }
        }
    }

    private void SelectOption()
    {
        isChoosing = false;
        optionsPanel.SetActive(false);

        if (currentOption == 0)
        {
            // SI
            if (!string.IsNullOrEmpty(escenaASi))
            {
                SceneManager.LoadScene(escenaASi);
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        didDialogueStart = false;
        isChoosing = false;

        dialoguePanel.SetActive(false);
        optionsPanel.SetActive(false);
        dialogueMark.SetActive(true);

        MovimientoJugador.canMove = true;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
        {
            dialogueMark.SetActive(true);
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
        {
            dialogueMark.SetActive(false);
            isPlayerInRange = false;
        }
    }
}