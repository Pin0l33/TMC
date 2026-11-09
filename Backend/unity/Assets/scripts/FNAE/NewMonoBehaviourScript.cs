using UnityEngine;
using UnityEngine.SceneManagement;
public class NewMonoBehaviourScript : MonoBehaviour
{
    void Update()
    {
        if (EntradaJuego.GetInteractDown())
        {
            SceneManager.LoadScene("FNAE_Menu");
        }
    }
}
