using System.Collections;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] private string targetScene;
    [SerializeField] private string targetSpawnId;

    [Header("Guardián de zona (opcional)")]
    [SerializeField] private BossGuardian guardian;
    [SerializeField] private string zoneId;
    [SerializeField, TextArea(2, 3)] private string challengeLineEs;
    [SerializeField, TextArea(2, 3)] private string challengeLineEn;

    [Header("UI")]
    [SerializeField] private GameObject arrowPrompt;
    [SerializeField] private float blinkInterval = 0.4f;

    private bool isPlayerInRange;
    private Coroutine blinkCoroutine;

    void Update()
    {
        {
    if (!MovimientoJugador.canMove) return;
}
        if (isPlayerInRange && EntradaJuego.GetInteractDown())
        {
            if (guardian == null || ZoneAccessManager.Instance.IsUnlocked(zoneId))
            {
                ProceedWithTransition();
            }
            else
            {
                HideArrow();
                string line = LocalizationManager.IsSpanish
                    ? challengeLineEs
                    : (!string.IsNullOrEmpty(challengeLineEn) ? challengeLineEn : challengeLineEs);
                ArduinoCompatibilityPrompt.Show("BossFight", () =>
                {
                    BattleManager.Instance.StartBattle(guardian, line, OnChallengeResult);
                }, () =>
                {
                    MovimientoJugador.canMove = true;
                });
            }
        }
    }

    private void OnChallengeResult(BattleState result)
    {
        if (result == BattleState.Won)
        {
            ZoneAccessManager.Instance.Unlock(zoneId);
            ProceedWithTransition();
        }
    }

    private void ProceedWithTransition()
    {
        HideArrow();
        if (IsMinigameTarget(targetScene))
        {
            ArduinoCompatibilityPrompt.Show(targetScene, () =>
            {
                DoSceneTransition();
            }, () =>
            {
                MovimientoJugador.canMove = true;
            });
        }
        else
        {
            DoSceneTransition();
        }
    }

    private bool IsMinigameTarget(string scene)
    {
        if (string.IsNullOrEmpty(scene)) return false;
        string s = scene.ToLowerInvariant();
        return s.Contains("taller") || s.Contains("fnae") || s.Contains("kiosco");
    }

    private void DoSceneTransition()
    {
        FadeController.Instance.PlayFadeOut(() =>
        {
            SceneTransitionManager.Instance.GoToScene(targetScene, targetSpawnId);
        });
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
