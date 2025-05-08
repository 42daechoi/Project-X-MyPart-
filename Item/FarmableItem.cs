using UnityEngine;

public class FarmableItem : MonoBehaviour
{
    public ItemData itemData;
    public int stackCount = 1;

    public void Pickup()
    {
        Item item = new Item(itemData, stackCount);
        bool success = InventoryManager.Instance.inventory.TryAddItem(item);
    }
}
