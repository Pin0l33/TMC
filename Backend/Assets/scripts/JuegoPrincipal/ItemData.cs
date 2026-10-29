using UnityEngine;

public enum ItemType
{
    Consumable,
    Cosmetic
}

[CreateAssetMenu(fileName = "NewItem", menuName = "TMC/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identidad")]
    public string id;
    public ItemType type;

    [Header("Tienda")]
    [Min(0)] public int price;

    [Header("Efecto")]
    [Min(0)] public int healAmount;

    [Header("Español")]
    [SerializeField] private string nameEs;
    [SerializeField, TextArea(2, 4)] private string descriptionEs;

    [Header("English")]
    [SerializeField] private string nameEn;
    [SerializeField, TextArea(2, 4)] private string descriptionEn;

    public string DisplayName
    {
        get
        {
            bool spanish = LocalizationManager.Instance == null ||
                           LocalizationManager.Instance.CurrentLanguage == Language.Spanish;
            return spanish ? nameEs : nameEn;
        }
    }

    public string Description
    {
        get
        {
            bool spanish = LocalizationManager.Instance == null ||
                           LocalizationManager.Instance.CurrentLanguage == Language.Spanish;
            return spanish ? descriptionEs : descriptionEn;
        }
    }
}
