using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image dialogueBoxImage;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject continueIndicator;

    [Header("Opciones")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private TMP_Text[] optionsText;

    private float typingTime = 0.05f;
    private bool isTyping;
    private bool isChoosing;
    private int currentOption;
    private int lineIndex;

    private ProfessorNPC currentNPC;
    private ProfessorNPC nearbyNPC;   // NUEVO: quién está en rango ahora mismo
    private Coroutine typingCoroutine;
    private Coroutine blinkCoroutine;

    public bool IsDialogueActive { get; private set; }

    void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void SetNearbyNPC(ProfessorNPC npc)      // NUEVO
    {
        nearbyNPC = npc;
    }

    public void ClearNearbyNPC(ProfessorNPC npc)    // NUEVO
    {
        if (nearbyNPC == npc) nearbyNPC = null;
    }

    void Update()
    {
        if (!IsDialogueActive)
        {
            // NUEVO: acá es el ÚNICO lugar donde se lee E para arrancar el diálogo
            if (nearbyNPC != null && Input.GetKeyDown(KeyCode.E))
            {
                var npc = nearbyNPC;
                StartDialogue(npc);
                npc.HideMark();
            }
            return;
        }

        if (isChoosing)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentOption = Mathf.Max(0, currentOption - 1);
                UpdateOptionUI();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentOption = Mathf.Min(currentNPC.Options.Length - 1, currentOption + 1);
                UpdateOptionUI();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                SelectOption();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentNPC.DialogueLines[lineIndex];
                isTyping = false;
                ShowContinueIndicator();
            }
            else
            {
                NextDialogueLine();
            }
        }
    }

    public void StartDialogue(ProfessorNPC npc)
    {
        if (IsDialogueActive) return;
        if (npc.DialogueLines.Length == 0) return;

        currentNPC = npc;
        IsDialogueActive = true;
        MovimientoJugador.canMove = false;

        dialogueBoxImage.sprite = npc.BoxSprite;
        dialoguePanel.SetActive(true);
        lineIndex = 0;

        typingCoroutine = StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        HideContinueIndicator();

        if (lineIndex < currentNPC.DialogueLines.Length)
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
        HideContinueIndicator();

        foreach (char ch in currentNPC.DialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }

        isTyping = false;
        ShowContinueIndicator();
    }

    private void ShowOptions()
    {
        if (currentNPC.Options.Length == 0)
        {
            EndDialogue();
            return;
        }

        isChoosing = true;
        dialoguePanel.SetActive(false);
        optionsPanel.SetActive(true);
        currentOption = 0;

        for (int i = 0; i < optionsText.Length; i++)
            optionsText[i].text = i < currentNPC.Options.Length ? currentNPC.Options[i] : "";

        UpdateOptionUI();
    }

    private void UpdateOptionUI()
    {
        for (int i = 0; i < currentNPC.Options.Length; i++)
            optionsText[i].text = (i == currentOption ? "> " : "  ") + currentNPC.Options[i];
    }

    private void SelectOption()
    {
        isChoosing = false;
        optionsPanel.SetActive(false);

        if (currentOption == 0 && !string.IsNullOrEmpty(currentNPC.EscenaASi))
            SceneManager.LoadScene(currentNPC.EscenaASi);
        else
            EndDialogue();
    }

    private void EndDialogue()
    {
        IsDialogueActive = false;
        isChoosing = false;
        HideContinueIndicator();
        dialoguePanel.SetActive(false);
        optionsPanel.SetActive(false);
        MovimientoJugador.canMove = true;
        currentNPC = null;
    }

    private void ShowContinueIndicator()
    {
        continueIndicator.SetActive(true);
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkIndicator());
    }

    private void HideContinueIndicator()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        continueIndicator.SetActive(false);
    }

    private IEnumerator BlinkIndicator()
    {
        var img = continueIndicator.GetComponent<Image>();
        while (true)
        {
            img.enabled = !img.enabled;
            yield return new WaitForSeconds(0.4f);
        }
    }
}