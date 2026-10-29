using System;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public static PlayerHP Instance { get; private set; }

    [SerializeField] private int maxHP = 20;
    public int CurrentHP { get; private set; }
    public int MaxHP => maxHP;

    public event Action<int, int> OnHPChanged;
    public event Action<int> OnDamaged;   // NUEVO
    public event Action OnPlayerDown;

    [SerializeField] private float invulnerabilityDuration = 0.5f;
    private float invulnerableTimer = 0f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        CurrentHP = maxHP;
    }

    void Update()
    {
        if (invulnerableTimer > 0f)
            invulnerableTimer -= Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        if (invulnerableTimer > 0f) return;

        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        invulnerableTimer = invulnerabilityDuration;
        OnHPChanged?.Invoke(CurrentHP, maxHP);
        OnDamaged?.Invoke(amount);   // NUEVO

        if (CurrentHP <= 0)
            OnPlayerDown?.Invoke();
    }

    public void Heal(int amount)   // NUEVO
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, maxHP);
        OnHPChanged?.Invoke(CurrentHP, maxHP);
    }

    public void ResetHP()
    {
        CurrentHP = maxHP;
        OnHPChanged?.Invoke(CurrentHP, maxHP);
    }
}