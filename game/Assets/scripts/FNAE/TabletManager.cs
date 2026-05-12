using UnityEngine;

public class TabletManager : MonoBehaviour
{
    public GameObject panelCamaras;

    private bool abierto = false;

    public GameManagerFNAF gameManager;

    public void ToggleTablet()
    {
        abierto = !abierto;

        panelCamaras.SetActive(abierto);

        if (abierto)
        {
            gameManager.AbrirCamaras();
        }
        else
        {
            gameManager.CerrarCamaras();
        }
    }
}