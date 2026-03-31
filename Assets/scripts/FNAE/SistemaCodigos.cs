using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SistemaCodigos : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text resultado;

    public void VerificarCodigo()
    {
        string codigo = inputField.text.ToLower().Trim();

        if (codigo == "lemos")
        {
            resultado.text = "Acceso concedido...";
            Invoke("IrAlVideo", 1.5f);
            Debug.Log("Codigo: [" + codigo + "]");
        }
        else
        {
            resultado.text = "Código incorrecto";
            Debug.Log("Codigo: [" + codigo + "]");
        }
    }

    void IrAlVideo()
    {
        SceneManager.LoadScene("Lemos");
    }
}