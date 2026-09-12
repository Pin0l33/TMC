using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private Transform player;

    private string pendingSpawnId;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void GoToScene(string sceneName, string spawnId)
    {
        pendingSpawnId = spawnId;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(pendingSpawnId)) return;

        foreach (var sp in FindObjectsOfType<SpawnPoint>())
        {
            if (sp.Id == pendingSpawnId)
            {
                player.position = sp.transform.position;
                break;
            }
        }
        pendingSpawnId = null;

        FadeController.Instance.PlayFadeIn();
    }
}