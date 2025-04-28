using UnityEngine;

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "GearData", menuName = "Scriptable Objects/GearData")]
public class GearData : ItemData
{
    [SerializeField] private Stats stats;
    [SerializeField] private ItemRarity rarity;
    [SerializeField, TextArea] private string specialAbilityDescription;

    public Stats GetStats() => stats;

    public ItemRarity GetRarity() => rarity;

    public string GetSpecialAbility()
    {
        return rarity == ItemRarity.Legendary ? specialAbilityDescription : null;
    }

    private void OnEnable()
    {
        isStackable = false;
    }
}