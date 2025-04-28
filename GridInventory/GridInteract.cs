using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemGrid))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryController inventoryController;
    private ItemGrid itemGrid;

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.selectedItemGrid = itemGrid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.selectedItemGrid = null;
    }

    private void Awake()
    {
        inventoryController = GetComponentInParent<InventoryController>();
        itemGrid = GetComponent<ItemGrid>();
    }
}
