using System.Collections;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] private string targetScene;
    [SerializeField] private string targetSpawnId;

    [Header("UI")]
    [SerializeField] private GameObject arrowPrompt;
    [SerializeField] private float blinkInterval = 0.4f;

    private bool isPlayerInRange;
    private Coroutine blinkCoroutine;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            HideArrow();
            FadeController.Instance.PlayFadeOut(() =>
            {
                SceneTransitionManager.Instance.GoToScene(targetScene, targetSpawnId);
            });
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador"))
        {
            isPlayerInRange = true;
            ShowArrow();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Jugador"))
        {
            isPlayerInRange = false;
            HideArrow();
        }
    }

    private void ShowArrow()
    {
        arrowPrompt.SetActive(true);
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkArrow());
    }

    private void HideArrow()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        arrowPrompt.SetActive(false);
    }

    private IEnumerator BlinkArrow()
    {
        var sr = arrowPrompt.GetComponent<SpriteRenderer>();
        while (true)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}