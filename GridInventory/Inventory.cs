using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private int gridWidth = 8;
    [SerializeField] private int gridHeight = 8;

    private Item[,] grid;
    private List<Item> itemList = new List<Item>();

    public List<Item> GetItemList() => itemList;
    public int GetGridWidth() => gridWidth;
    public int GetGridHeight() => gridHeight;

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
        grid = new Item[gridWidth, gridHeight];
    }

    /// <summary>
    /// �������� ������ ��ġ�� ��ġ�� �� �ִ��� Ȯ��
    /// </summary>
    private bool CanPlaceItem(Vector2Int position, Item item)
    {
        foreach (var cell in item.Shape)
        {
            int checkX = position.x + cell.x;
            int checkY = position.y + cell.y;

            if (checkX < 0 || checkY < 0 || checkX >= gridWidth || checkY >= gridHeight)
                return false;

            if (grid[checkX, checkY] != null)
                return false;
        }

        return true;
    }

    /// <summary>
    /// �������� ������ ��ġ�� ��ġ
    /// </summary>
    public bool PlaceItem(Vector2Int position, Item item)
    {
        if (!CanPlaceItem(position, item))
            return false;

        foreach (var cell in item.Shape)
        {
            int placeX = position.x + cell.x;
            int placeY = position.y + cell.y;

            grid[placeX, placeY] = item;
        }

        item.posX = position.x;
        item.posY = position.y;
        itemList.Add(item);

        Debug.Log($"Inventory : ������ {item.data.itemName}�� ({position.x}, {position.y})�� ��ġ�߽��ϴ�.");
        //PrintGrid();
        EventBus.OnChangeInventory?.Invoke();
        return true;
    }


    /// <summary>
    /// �Ҹ�ǰ ������� ���� ������ ���� ���� �� ����
    /// </summary>
    public bool DecreaseItemStack(string itemName)
    {
        foreach (Item item in itemList)
        {
            if (item.data.name == itemName)
            {
                item.currentStack--;
                if (item.currentStack == 0)
                {
                    RemoveItemAt(new Vector2Int(item.posX, item.posY));
                }
                else
                {
                    EventBus.OnChangeInventory?.Invoke();
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// �������� �ڵ����� ��ġ�� �� �ִ� ��ġ�� ã�� ��ġ
    /// </summary>
    private bool TryAutoPlaceItem(Item item)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (CanPlaceItem(position, item))
                {
                    return PlaceItem(position, item);
                }
            }
        }
        return false;
    }

    /// <summary>
    /// ���ġ Ȥ�� �ǸŸ� ���� ������ ��ġ�� �������� ����
    /// </summary>
    public void RemoveItemAt(Vector2Int position, bool isReplace = false)
    {
        Item item = GetItemAt(position);
        if (item == null) return;

        foreach (var cell in item.Shape)
        {
            int cellX = item.posX + cell.x;
            int cellY = item.posY + cell.y;
            if (cellX >= 0 && cellY >= 0 && cellX < gridWidth && cellY < gridHeight)
                grid[cellX, cellY] = null;
        }

        itemList.Remove(item);
        if (!isReplace) EventBus.OnChangeInventory?.Invoke();
        Debug.Log($"Inventory: ���ġ�� ���� ({position.x}, {position.y}) ��ġ�� ������ {item.data.itemName} ���ŵ�.");
    }

    /// <summary>
    /// ������ �������� ������
    /// </summary>
    public void DropItem(Item item)
    {
        if (item == null) return;

        foreach (var cell in item.Shape)
        {
            int cellX = item.posX + cell.x;
            int cellY = item.posY + cell.y;
            if (cellX >= 0 && cellY >= 0 && cellX < gridWidth && cellY < gridHeight)
                grid[cellX, cellY] = null;
        }

        item.SetSlotUI(null);
        itemList.Remove(item);
        EventBus.OnChangeInventory?.Invoke();
        Debug.Log($"Inventory: ({item.posX}, {item.posY}) ��ġ�� ������ {item.data.itemName} ������ �õ�.");
    }

    /// <summary>
    /// �������� �߰��� �� �ִ��� Ȯ���ϰ� �߰�
    /// </summary>
    public bool TryAddItem(Item item)
    {
        if (item.data.isStackable)
        {
            foreach (var existItem in itemList)
            {
                if (existItem.CanStackWith(item))
                {
                    int added = existItem.AddToStack(item.currentStack);
                    Debug.Log($"Inventory : {item.data.itemName}�� {existItem.currentStack}���� �����Ǿ����ϴ�.");
                    item.currentStack -= added;
                    EventBus.OnChangeInventory?.Invoke();
                    if (item.currentStack <= 0) return true;
                }
            }
        }

        return TryAutoPlaceItem(item);
    }

    /// <summary>
    /// ������ ��ġ�� �ִ� �������� ��ȯ
    /// </summary>
    public Item GetItemAt(Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x >= gridWidth || position.y >= gridHeight)
            return null;

        return grid[position.x, position.y];
    }

    /// <summary>
    /// �׸��带 ����Ͽ� ���� ���¸� �ֿܼ� ���
    /// </summary>
    public void PrintGrid()
    {
        string output = "";

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Item item = grid[x, y];
                if (item == null)
                {
                    output += ". ";
                }
                else
                {
                    output += item.data.itemName[0] + " ";
                }
            }
            output += "\n";
        }

        Debug.Log(output);
    }

    /// <summary>
    /// ��� ��� �������� �� �Ӽ� ���� ��ȯ
    /// </summary>
    public Stats GetTotalGearStats()
    {
        Stats totalStats = Stats.Zero;

        foreach (var item in itemList)
        {
            if (item.data is GearData gearData)
            {
                totalStats += gearData.GetStats();
            }
        }
        return totalStats;
    }


    /// <summary>
    /// �κ��丮 Ȯ�� �Լ�
    /// </summary>
    public void ExpandInventory()
    {
        int prevWidth = gridWidth;
        int prevHeight = gridHeight;
        Item[,] oldGrid = grid;

        if (gridWidth == 8 && gridHeight == 8)
        {
            gridWidth = 10;
            gridHeight = 8;
        }
        else if (gridWidth == 10 && gridHeight == 8)
        {
            gridWidth = 10;
            gridHeight = 10;
        }

        grid = new Item[gridWidth, gridHeight];


        foreach (var item in itemList)
        {
            foreach (var cell in item.Shape)
            {
                int x = item.posX + cell.x;
                int y = item.posY + cell.y;

                grid[x, y] = item;
            }
        }

        EventBus.OnExpandInventory?.Invoke();
    }
}
