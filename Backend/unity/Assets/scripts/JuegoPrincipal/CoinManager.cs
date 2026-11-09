using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    private const string PrefsKey = "Coins";
    [SerializeField, Min(0)] private int startingCoins = 0;
    [SerializeField] private bool enableTestKey = true;

    public int Coins { get; private set; }
    public event Action OnCoinsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Coins = Mathf.Max(0, PlayerPrefs.GetInt(PrefsKey, startingCoins));
        OnCoinsChanged?.Invoke();
    }

    private void Update()
    {
        // Atajo de prueba: suma 1 moneda por pulsación de M.
        if (enableTestKey && Input.GetKeyDown(KeyCode.M))
            Add(1);
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
        Save();
    }

    public bool Spend(int amount)
    {
        if (amount <= 0 || Coins < amount) return false;
        Coins -= amount;
        Save();
        return true;
    }

    public void SetCoins(int amount)
    {
        Coins = Mathf.Max(0, amount);
        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetInt(PrefsKey, Coins);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke();
    }
}
