using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    private readonly List<Selectable> mainSelectables = new List<Selectable>();
    private GameObject optionsPanel;
    private Selectable optionsButton;
    private Selectable optionsFirst;
    private bool optionsOpen;

    void Awake()
    {
        LocalizationManager.EnsureInitialized();
    }

    void Start()
    {
        Transform root = transform.root;
        optionsPanel = FindChild(root, "PanelOpciones")?.gameObject;
        optionsButton = FindChild(root, "Opciones")?.GetComponent<Selectable>();

        AddMainSelectable(root, "Jugar");
        AddMainSelectable(root, "Opciones");
        AddMainSelectable(root, "Salir");

        if (optionsPanel != null)
        {
            Selectable[] selectables = optionsPanel.GetComponentsInChildren<Selectable>(true);
            for (int i = 0; i < selectables.Length; i++)
            {
                if (selectables[i].name == "volver")
                {
                    optionsFirst = selectables[i];
                    break;
                }
            }
            if (optionsFirst == null && selectables.Length > 0)
                optionsFirst = selectables[0];
        }

        optionsOpen = optionsPanel != null && optionsPanel.activeInHierarchy;
        if (optionsOpen) ApplyOptionsState(true);
    }

    void Update()
    {
        if (optionsPanel == null) return;

        bool open = optionsPanel.activeInHierarchy;
        if (open == optionsOpen) return;

        optionsOpen = open;
        ApplyOptionsState(open);
    }

    private void ApplyOptionsState(bool open)
    {
        SetMainMenuInteractable(!open);

        if (EventSystem.current == null) return;

        Selectable target = open ? optionsFirst : optionsButton;
        if (target != null && target.IsActive() && target.IsInteractable())
            EventSystem.current.SetSelectedGameObject(target.gameObject);
    }

    private void SetMainMenuInteractable(bool interactable)
    {
        for (int i = 0; i < mainSelectables.Count; i++)
        {
            if (mainSelectables[i] != null)
                mainSelectables[i].interactable = interactable;
        }
    }

    private void AddMainSelectable(Transform root, string name)
    {
        Transform child = FindChild(root, name);
        if (child == null) return;

        Selectable selectable = child.GetComponent<Selectable>();
        if (selectable != null)
            mainSelectables.Add(selectable);
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

    public void jugarConAnimacion(BotonSpriteAnimado boton)
    {
        boton.OnPointerClick(null);
    }

    public void salir()
    {
        Application.Quit();
    }
}
