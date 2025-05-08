using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject skillInventoryPanel;
    [SerializeField] private ItemGrid skillItemGrid;
    [SerializeField] private ItemGrid itemGrid;

    public Inventory inventory { get; private set; }
    public SkillTreeInventory skillTreeInventory { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitInventories();
        InitGrid();
    }

    private void InitInventories()
    {
        var inventories = GetComponentsInChildren<Inventory>(true);
        foreach (var inv in inventories)
        {
            if (inv is SkillTreeInventory)
            {
                skillTreeInventory = inv as SkillTreeInventory;
                skillTreeInventory.Init();
            }

            else
            {
                inventory = inv;
                inventory.Init();
            }
        }
    }

    private void InitGrid()
    {
        var grids = GetComponentsInChildren<ItemGrid>(true);
        foreach (var grid in grids)
        {
            grid.Init();
        }
    }

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
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ToggleSkillInventory();
        }
    }

    private void ToggleSkillInventory()
    {
        bool isActive = !skillInventoryPanel.activeSelf;
        skillInventoryPanel.SetActive(isActive);

        if (isActive)
        {
            skillItemGrid.UpdateGrid();
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
