using UnityEngine;

[CreateAssetMenu(fileName = "ComsumableItemData", menuName = "Scriptable Objects/ComsumableItemData")]
public class ComsumableItemData : ItemData
{
    [SerializeField] private StatBinder effect;

    public StatBinder GetStatBinder() => effect;

    private void OnEnable()
    {
        isStackable = true;
    }
}
