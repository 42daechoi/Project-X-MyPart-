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
    /// 아이템을 지정된 위치에 배치할 수 있는지 확인
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
    /// 아이템을 지정된 위치에 배치
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

        Debug.Log($"Inventory : 아이템 {item.data.itemName}을 ({position.x}, {position.y})에 위치했습니다.");
        //PrintGrid();
        EventBus.OnChangeInventory?.Invoke();
        return true;
    }


    /// <summary>
    /// 소모품 사용으로 인한 아이템 스택 감소 및 삭제
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
    /// 아이템을 자동으로 배치할 수 있는 위치를 찾고 배치
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
    /// 재배치 혹은 판매를 위해 지정된 위치의 아이템을 제거
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
        Debug.Log($"Inventory: 재배치를 위해 ({position.x}, {position.y}) 위치의 아이템 {item.data.itemName} 제거됨.");
    }

    /// <summary>
    /// 지정된 아이템을 버리기
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
        Debug.Log($"Inventory: ({item.posX}, {item.posY}) 위치의 아이템 {item.data.itemName} 버리기 시도.");
    }

    /// <summary>
    /// 아이템을 추가할 수 있는지 확인하고 추가
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
                    Debug.Log($"Inventory : {item.data.itemName}이 {existItem.currentStack}개로 누적되었습니다.");
                    item.currentStack -= added;
                    EventBus.OnChangeInventory?.Invoke();
                    if (item.currentStack <= 0) return true;
                }
            }
        }

        return TryAutoPlaceItem(item);
    }

    /// <summary>
    /// 지정된 위치에 있는 아이템을 반환
    /// </summary>
    public Item GetItemAt(Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x >= gridWidth || position.y >= gridHeight)
            return null;

        return grid[position.x, position.y];
    }

    /// <summary>
    /// 그리드를 출력하여 현재 상태를 콘솔에 출력
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
    /// 모든 장비 아이템의 총 속성 값을 반환
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
    /// 인벤토리 확장 함수
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
