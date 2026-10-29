using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private ItemData[] allItems;
    private const string PrefsKey = "Inventory";

    [Serializable]
    private class InventoryEntry
    {
        public string id;
        public int count;
    }

    [Serializable]
    private class InventorySave
    {
        public List<InventoryEntry> items = new List<InventoryEntry>();
    }

    private readonly Dictionary<string, int> counts = new Dictionary<string, int>();
    private readonly Dictionary<string, ItemData> byId = new Dictionary<string, ItemData>();

    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        byId.Clear();
        if (allItems != null)
        {
            foreach (ItemData item in allItems)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.id))
                    continue;

                if (byId.ContainsKey(item.id))
                    Debug.LogWarning($"InventoryManager: ID duplicado '{item.id}'. Se usará el último.", this);

                byId[item.id] = item;
            }
        }

        Load();
    }

    public int Count(string id) =>
        !string.IsNullOrEmpty(id) && counts.TryGetValue(id, out int count) ? count : 0;

    public bool Has(string id) => Count(id) > 0;

    public ItemData GetItem(string id) =>
        !string.IsNullOrEmpty(id) && byId.TryGetValue(id, out ItemData item) ? item : null;

    public void Add(ItemData item, int amount = 1)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.id))
        {
            Debug.LogWarning("InventoryManager.Add recibió un ítem nulo o sin ID.", this);
            return;
        }

        if (amount <= 0) return;

        // Registra el ítem también si fue añadido después de inicializar el manager.
        byId[item.id] = item;
        counts[item.id] = Count(item.id) + amount;
        Save();
    }

    public bool Consume(string id, int amount = 1)
    {
        if (amount <= 0) return false;

        int current = Count(id);
        if (current < amount) return false;

        int remaining = current - amount;
        if (remaining == 0) counts.Remove(id);
        else counts[id] = remaining;

        Save();
        return true;
    }

    public List<ItemData> GetOwnedConsumables()
    {
        var result = new List<ItemData>();
        foreach (KeyValuePair<string, int> entry in counts)
        {
            if (entry.Value <= 0) continue;
            ItemData item = GetItem(entry.Key);
            if (item != null && item.type == ItemType.Consumable)
                result.Add(item);
        }

        result.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.CurrentCulture));
        return result;
    }

    public void ClearInventory()
    {
        counts.Clear();
        Save();
    }

    private void Save()
    {
        var save = new InventorySave();
        foreach (KeyValuePair<string, int> entry in counts)
        {
            if (entry.Value > 0)
                save.items.Add(new InventoryEntry { id = entry.Key, count = entry.Value });
        }

        PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(save));
        PlayerPrefs.Save();
        OnInventoryChanged?.Invoke();
    }

    private void Load()
    {
        counts.Clear();
        string json = PlayerPrefs.GetString(PrefsKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                InventorySave save = JsonUtility.FromJson<InventorySave>(json);
                if (save != null && save.items != null)
                {
                    foreach (InventoryEntry entry in save.items)
                    {
                        if (entry == null || string.IsNullOrWhiteSpace(entry.id) ||
                            entry.count <= 0 || !byId.ContainsKey(entry.id))
                            continue;

                        counts[entry.id] = entry.count;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"InventoryManager: no se pudo cargar el inventario: {ex.Message}", this);
            }
        }

        OnInventoryChanged?.Invoke();
    }
}
