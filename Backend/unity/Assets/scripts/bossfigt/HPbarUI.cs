using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text hpText;

    void OnEnable()
    {
        PlayerHP.Instance.OnHPChanged += UpdateBar;
        UpdateBar(PlayerHP.Instance.CurrentHP, GetMaxHP());
    }

    void OnDisable()
    {
        if (PlayerHP.Instance != null)
            PlayerHP.Instance.OnHPChanged -= UpdateBar;
    }

    private int GetMaxHP()
    {
        // Truco simple: como CurrentHP arranca en maxHP, lo usamos de referencia al activarse
        return PlayerHP.Instance.CurrentHP;
    }

    private void UpdateBar(int current, int max)
    {
        fillImage.fillAmount = (float)current / max;
        hpText.text = $"{current} / {max}";
    }
}