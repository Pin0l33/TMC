using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoTransicion : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string siguienteEscena = "Noche1";

    void Start()
    {
        // Cuando termina el video, ejecuta la función
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(siguienteEscena);
    }
}