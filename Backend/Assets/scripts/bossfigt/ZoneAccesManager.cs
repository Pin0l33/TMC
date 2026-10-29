using System.Collections.Generic;
using UnityEngine;

public class ZoneAccessManager : MonoBehaviour
{
    public static ZoneAccessManager Instance { get; private set; }

    private HashSet<string> unlockedZones = new HashSet<string>();
    private const string PrefsKey = "UnlockedZones";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        Load();
    }

    public bool IsUnlocked(string zoneId) => unlockedZones.Contains(zoneId);

    public void Unlock(string zoneId)
    {
        if (unlockedZones.Add(zoneId))
            Save();
    }

    private void Load()
    {
        string saved = PlayerPrefs.GetString(PrefsKey, "");
        if (!string.IsNullOrEmpty(saved))
            unlockedZones = new HashSet<string>(saved.Split(','));
    }

    private void Save()
    {
        PlayerPrefs.SetString(PrefsKey, string.Join(",", unlockedZones));
        PlayerPrefs.Save();
    }
}