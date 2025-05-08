using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 50;
    public const float tileSizeHeight = 50;
    private int gridWidth;
    private int gridHeight;

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    private RectTransform rectTransform;
    private Canvas rootCanvas;
    [SerializeField] private GameObject ItemUIPrefab;
    public Inventory inventory;
    public EInventoryType inventoryType;

    private Queue<GameObject> itemObjectPool = new Queue<GameObject>();
    private List<GameObject> activeItemObjectList = new List<GameObject>();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
        InitGrid();
    }

    public void Init()
    {
        if (inventoryType == EInventoryType.SkillTree)
        {
            inventory = InventoryManager.Instance.skillTreeInventory;
        }
        else
        {
            inventory = InventoryManager.Instance.inventory;
        }
    }

    private void OnEnable()
    {
        EventBus.OnChangeInventory += UpdateGrid;
        EventBus.OnExpandInventory += InitGrid;
        InitGrid();
    }

    private void OnDisable()
    {
        EventBus.OnChangeInventory -= UpdateGrid;
        EventBus.OnExpandInventory -= InitGrid;
    }

    /// <summary>
    /// �׸��� ����� Inventory ������ ������� �ʱ�ȭ
    /// </summary>
    private void InitGrid()
    {
        gridWidth = inventory.GetGridWidth();
        gridHeight = inventory.GetGridHeight();
        Vector2 size = new Vector2(gridWidth * tileSizeWidth, gridHeight * tileSizeWidth);
        rectTransform.sizeDelta = size;
    }

    /// <summary>
    /// ���콺 ȭ�� ��ǥ�� �׸��� ��ǥ�� ��ȯ
    /// </summary>
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        tileGridPosition.x = (int)(positionOnTheGrid.x / (tileSizeWidth * rootCanvas.scaleFactor));
        tileGridPosition.y = (int)(positionOnTheGrid.y / (tileSizeHeight * rootCanvas.scaleFactor));

        return tileGridPosition;
    }

    /// <summary>
    /// ��� �κ��丮 �����ۿ� ���� UI ����
    /// </summary>
    public void UpdateGrid()
    {
        foreach (var itemObj in activeItemObjectList)
        {
            itemObj.SetActive(false);
            itemObjectPool.Enqueue(itemObj);
        }
        activeItemObjectList.Clear();

        int count = 0;
        foreach (Item item in inventory.GetItemList())
        {
            count++;
            GameObject itemObj = GetObjectFromPool();
            itemObj.SetActive(true);
            activeItemObjectList.Add(itemObj);

            InventorySlotUI slotUI = itemObj.GetComponent<InventorySlotUI>();
            slotUI.SetItem(item, rootCanvas);

            Vector2 position = CalculatePositionOnGrid(item);
            RectTransform itemRect = itemObj.GetComponent<RectTransform>();
            itemRect.localPosition = position;
        }
    }

    /// <summary>
    /// Ǯ���� ������Ʈ�� �������ų� ���� ����
    /// </summary>
    private GameObject GetObjectFromPool()
    {
        if (itemObjectPool.Count > 0)
        {
            return itemObjectPool.Dequeue();
        }
        else
        {
            return Instantiate(ItemUIPrefab, rectTransform);
        }
    }

    /// <summary>
    /// ������ UI ��ġ
    /// �������� ���� ��ġ�� �������� �׸��� �� UI ��ġ ���
    /// </summary>
    public Vector2 CalculatePositionOnGrid(Item item)
    {
        Vector2 position = new Vector2();

        Vector2Int boundingSize = item.BoundingSize;
        position.x = item.posX * tileSizeWidth + tileSizeWidth * boundingSize.x / 2;
        position.y = -(item.posY * tileSizeHeight + tileSizeHeight * boundingSize.y / 2);
        return position;
    }

    /// <summary>
    /// ���̶���Ʈ�� ����� ���� ���� �߽� UI ��ġ�� ���
    /// </summary>
    public Vector2 CalculateCellPosition(int x, int y)
    {
        Vector2 position = new Vector2();
        position.x = x * tileSizeWidth + tileSizeWidth / 2;
        position.y = -(y * tileSizeHeight + tileSizeHeight / 2);
        return position;
    }
}
