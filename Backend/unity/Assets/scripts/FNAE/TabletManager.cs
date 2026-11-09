using UnityEngine;
using TMPro;

public class TabletManager : MonoBehaviour
{
    public GameObject panelCamaras;

    private bool abierto = false;

    public GameManagerFNAF gameManager;
    private TMP_Text textoBotonTablet;

    private void Start()
    {
        GameObject boton = GameObject.Find("BotonTablet");
        if (boton != null)
            textoBotonTablet = boton.GetComponentInChildren<TMP_Text>(true);
        ActualizarTextoBoton();
    }

    public void ToggleTablet()
    {
        abierto = !abierto;

        panelCamaras.SetActive(abierto);
        ActualizarTextoBoton();

        if (abierto)
        {
            gameManager.AbrirCamaras();
        }
        else
        {
            gameManager.CerrarCamaras();
        }
    }

    private void ActualizarTextoBoton()
    {
        if (textoBotonTablet != null)
            textoBotonTablet.text = abierto ? "CERRAR CÁMARAS" : "ABRIR CÁMARAS";
    }
}
