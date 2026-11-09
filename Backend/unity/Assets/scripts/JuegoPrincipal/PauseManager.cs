using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private readonly List<Selectable> pauseSelectables = new List<Selectable>();
    private GameObject optionsPanel;
    private Selectable continueButton;
    private Selectable optionsButton;
    private Selectable optionsFirst;
    private bool isPaused;
    private bool optionsOpen;

    private TMP_Text pauseTitleText;
    private TMP_Text continueText;
    private TMP_Text optionsText;
    private TMP_Text exitText;

    void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateLanguageTexts;
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateLanguageTexts;
    }

    private void UpdateLanguageTexts(Language lang)
    {
        bool es = lang == Language.Spanish;
        if (pauseTitleText != null) pauseTitleText.text = es ? "PAUSA" : "PAUSE";
        if (continueText != null) continueText.text = es ? "CONTINUAR" : "CONTINUE";
        if (optionsText != null) optionsText.text = es ? "OPCIONES" : "OPTIONS";
        if (exitText != null) exitText.text = es ? "SALIR AL MENÚ" : "EXIT TO MENU";
    }

    void Start()
    {
        Transform root = transform.root;

        if (pausePanel == null)
            pausePanel = FindChild(root, "PausePanel")?.gameObject;

        optionsPanel = FindChild(root, "PanelOpciones")?.gameObject;
        continueButton = FindChild(root, "ContinueButton")?.GetComponent<Selectable>();
        optionsButton = FindChild(root, "OptionsButton")?.GetComponent<Selectable>();

        pauseTitleText = FindChild(root, "PauseTitle")?.GetComponent<TMP_Text>();
        if (continueButton != null) continueText = continueButton.GetComponentInChildren<TMP_Text>(true);
        if (optionsButton != null) optionsText = optionsButton.GetComponentInChildren<TMP_Text>(true);
        Transform exitBtn = FindChild(root, "ExitButton");
        if (exitBtn != null) exitText = exitBtn.GetComponentInChildren<TMP_Text>(true);

        UpdateLanguageTexts(LocalizationManager.CurrentSafeLanguage);

        if (optionsPanel != null)
        {
            optionsFirst = FindChild(optionsPanel.transform, "volver")?.GetComponent<Selectable>();
            if (optionsFirst == null)
            {
                Selectable[] selectables = optionsPanel.GetComponentsInChildren<Selectable>(true);
                if (selectables.Length > 0)
                    optionsFirst = selectables[0];
            }
        }

        if (pausePanel != null)
        {
            pauseSelectables.AddRange(pausePanel.GetComponentsInChildren<Selectable>(true));

            Transform pauseButton = FindChild(root, "PauseButton");
            if (pauseButton != null)
            {
                Selectable selectable = pauseButton.GetComponent<Selectable>();
                if (selectable != null && !pauseSelectables.Contains(selectable))
                    pauseSelectables.Add(selectable);
            }
        }

        isPaused = pausePanel != null && pausePanel.activeInHierarchy;
        optionsOpen = optionsPanel != null && optionsPanel.activeInHierarchy;

        if (isPaused)
        {
            SetPauseInteractable(!optionsOpen);
            Select(optionsOpen ? optionsFirst : continueButton);
        }
    }

    void Update()
    {
        if (pausePanel == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && optionsOpen)
            {
                CloseOptions();
                return;
            }

            TogglePause();
            return;
        }

        if (optionsPanel == null) return;

        bool open = optionsPanel.activeInHierarchy;
        if (open == optionsOpen) return;

        optionsOpen = open;
        if (!isPaused) return;

        SetPauseInteractable(!open);
        Select(open ? optionsFirst : optionsButton);
    }

    public void TogglePause()
    {
        bool opening = !isPaused;
        isPaused = opening;
        pausePanel.SetActive(opening);
        Time.timeScale = opening ? 0f : 1f;

        bool canPlayerMove = !opening;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            canPlayerMove = false;
        if (ArduinoCompatibilityPrompt.Instance != null && ArduinoCompatibilityPrompt.Instance.IsPromptActive)
            canPlayerMove = false;

        MovimientoJugador.canMove = canPlayerMove;

        if (optionsPanel != null && optionsPanel.activeInHierarchy)
        {
            optionsPanel.SetActive(false);
            optionsOpen = false;
        }

        SetPauseInteractable(true);

        if (opening)
            Select(continueButton);
        else
            ClearSelection();
    }

    private void CloseOptions()
    {
        if (optionsPanel == null) return;

        optionsPanel.SetActive(false);
        optionsOpen = false;
        SetPauseInteractable(true);
        Select(optionsButton);
    }

    private void SetPauseInteractable(bool interactable)
    {
        for (int i = 0; i < pauseSelectables.Count; i++)
        {
            if (pauseSelectables[i] != null)
                pauseSelectables[i].interactable = interactable;
        }
    }

    private static void Select(Selectable target)
    {
        if (EventSystem.current == null) return;
        if (target != null && target.IsActive() && target.IsInteractable())
            EventSystem.current.SetSelectedGameObject(target.gameObject);
    }

    private static void ClearSelection()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private static Transform FindChild(Transform parent, string name)
    {
        if (parent == null) return null;
        if (parent.name == name) return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform result = FindChild(parent.GetChild(i), name);
            if (result != null) return result;
        }

        return null;
    }

    public void OnContinueButton() => TogglePause();

    public void OnExitToMenuButton()
    {
        isPaused = false;
        optionsOpen = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        SetPauseInteractable(true);
        ClearSelection();

        Time.timeScale = 1f;
        MovimientoJugador.canMove = true;
        SceneManager.LoadScene("MenuPrincipal");
    }
}