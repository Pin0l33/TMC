using UnityEngine; 
using UnityEngine.SceneManagement;
 
public class BootLoader : MonoBehaviour

{ [SerializeField] private string firstScene = "juego"; 

void Start() { SceneManager.LoadScene(firstScene, LoadSceneMode.Single); } }