using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private ItemGrid itemGrid;

    private void OnEnable()
    {
        EventBus.OnInteractMerchant += ActiveInventory;
    }

    private void OnDisable()
    {
        EventBus.OnInteractMerchant -= ActiveInventory;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        if (isActive)
        {
            itemGrid.UpdateGrid();
        }
    }

    private void ActiveInventory(string merchatType, bool isInteract)
    {
        if (isInteract == true && !inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            itemGrid.UpdateGrid();
        }
        if (isInteract == false && inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
        }
    }
}
