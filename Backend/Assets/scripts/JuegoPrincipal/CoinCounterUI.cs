using TMPro;
using UnityEngine;

public class CoinCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countText;

    private void OnEnable()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.OnCoinsChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.OnCoinsChanged -= Refresh;
    }

    private void Refresh()
    {
        if (countText == null) return;
        countText.text = CoinManager.Instance != null
            ? CoinManager.Instance.Coins.ToString()
            : "0";
    }
}
