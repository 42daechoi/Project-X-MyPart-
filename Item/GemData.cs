using UnityEngine;

public enum GemSize
{
    Small,
    Medium,
    Large
}

[CreateAssetMenu(fileName = "GemData", menuName = "Scriptable Objects/GemData")]
public class GemData : ItemData
{
    [SerializeField] private GemSize gemSize;
    [SerializeField] private WeaponType weaponType;
    public GemSize GemSize => gemSize;

    private void OnEnable()
    {
        isStackable = true;

        maxStackSize = gemSize switch
        {
            GemSize.Small => 99,
            GemSize.Medium => 50,
            GemSize.Large => 10,
            _ => 1
        };
    }
}
