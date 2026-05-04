using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BotonFNAF : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private TMP_Text texto;

    public string accion;
    private string textoOriginal;

    void Start()
    {
        texto = GetComponent<TMP_Text>();
        textoOriginal = texto.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        texto.text = ">> " + textoOriginal;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        texto.text = textoOriginal;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (accion == "NewGame")
        {
            SceneManager.LoadScene("Diario");
        }
        else if (accion == "Continue")
        {
            Debug.Log("Continuar (todavía no implementado)");
        }
        else if (accion == "Codes")
        {
            SceneManager.LoadScene("CodesScene");
        }
        else if (accion == "Exit")
        {
            SceneManager.LoadScene("Mapa");
        }
    }
}