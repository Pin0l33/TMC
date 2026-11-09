using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotonFNAF : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler, ISubmitHandler, IMoveHandler, IArduinoMenuItem
{
    private static readonly List<BotonFNAF> navigationItems = new List<BotonFNAF>();
    private static int testKeyHandledFrame = -1;
    private TMP_Text texto;
    private Image fondo;
    private bool isSelected;
    private bool isHovered;
    private bool isPressed;
    private bool actionStarted;

    public string accion;
    private string textoOriginal;

    public GameObject MenuObject => gameObject;

    public void OnArduinoSelectionChanged(bool selected)
    {
        isSelected = selected;
        RefreshVisual();
    }

    public void OnArduinoSubmit() => OnPointerClick(null);
    public void OnSelect(BaseEventData eventData) => OnArduinoSelectionChanged(true);
    public void OnDeselect(BaseEventData eventData) => OnArduinoSelectionChanged(false);
    public void OnSubmit(BaseEventData eventData) => OnArduinoSubmit();

    public void OnMove(AxisEventData eventData)
    {
        if (eventData == null || (eventData.moveDir != MoveDirection.Up && eventData.moveDir != MoveDirection.Down))
            return;

        BuildNavigationList();
        if (navigationItems.Count < 2) return;

        int index = navigationItems.IndexOf(this);
        if (index < 0) return;

        int nextIndex = eventData.moveDir == MoveDirection.Up ? index - 1 : index + 1;
        nextIndex = Mathf.Clamp(nextIndex, 0, navigationItems.Count - 1);
        if (nextIndex == index) return;

        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null) return;

        eventData.Use();
        eventSystem.SetSelectedGameObject(navigationItems[nextIndex].gameObject);
    }

    void Awake()
    {
        texto = GetComponent<TMP_Text>();
        textoOriginal = texto.text;
        Transform fondoTransform = transform.parent != null ? transform.parent.Find("Fondo " + gameObject.name) : null;
        if (fondoTransform != null)
            fondo = fondoTransform.GetComponent<Image>();

        if (accion == "Continue")
        {
            FNAEProgression.EnsureInitialized();
            textoOriginal = "CONTINUAR · NOCHE " + FNAEProgression.MaxUnlockedNight;
            texto.text = textoOriginal;
        }
    }

    void Start()
    {
        RefreshVisual();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (testKeyHandledFrame == Time.frameCount) return;
        // Atajo de pruebas exclusivo del Editor: F1-F5 inicia directamente la noche elegida.
        for (int night = 1; night <= 5; night++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.F1 + night - 1)))
            {
                testKeyHandledFrame = Time.frameCount;
                FNAEProgression.StartTestNight(night);
                return;
            }
        }
    }
#endif

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        RefreshVisual();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isPressed = false;
        RefreshVisual();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (actionStarted) return;
        actionStarted = true;

        if (accion == "NewGame")
        {
            ArduinoCompatibilityPrompt.Show("FNAE", () =>
            {
                FNAEProgression.StartNewGame();
                SceneManager.LoadScene("Diario");
            }, () =>
            {
                actionStarted = false;
            });
        }
        else if (accion == "Continue")
        {
            ArduinoCompatibilityPrompt.Show("FNAE", () =>
            {
                FNAEProgression.ContinueUnlockedNight();
                SceneManager.LoadScene("Diario");
            }, () =>
            {
                actionStarted = false;
            });
        }
        else if (accion == "Codes")
        {
            SceneManager.LoadScene("CodesScene");
        }
        else if (accion == "Exit")
        {
            SceneManager.LoadScene("juego");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        RefreshVisual();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        RefreshVisual();
    }

    private void BuildNavigationList()
    {
        navigationItems.Clear();
        BotonFNAF[] items = FindObjectsByType<BotonFNAF>(FindObjectsSortMode.None);
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].isActiveAndEnabled)
                navigationItems.Add(items[i]);
        }

        navigationItems.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
    }

    private void RefreshVisual()
    {
        if (texto != null)
            texto.text = isSelected || isHovered ? ">> " + textoOriginal : textoOriginal;

        if (fondo == null) return;

        if (isPressed)
            fondo.color = new Color(0.24f, 0.12f, 0.12f, 1f);
        else if (isSelected || isHovered)
            fondo.color = new Color(0.18f, 0.28f, 0.29f, 0.98f);
        else
            fondo.color = new Color(0.075f, 0.11f, 0.13f, 0.94f);
    }
}
