using UnityEngine;
using UnityEngine.SceneManagement;

public class Volver : MonoBehaviour
{

    void Update()
    {
        if (EntradaJuego.GetInteractDown())
        {
           SceneManager.LoadScene("FNAE_Menu");
        }
        
    }
}
