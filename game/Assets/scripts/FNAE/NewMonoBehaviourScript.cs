using UnityEngine;
using UnityEngine.SceneManagement;
public class volver : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("FNAE_Menu");
        }
    }
}
