using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class SkillTreeInventory : Inventory
{
    public SkillInventoryLayout layout;
    public ItemData startCellData;
    
    private ECellType[,] cellTypes;
    private Dictionary<Vector2Int, CellData> CellDataMap = new();
    [HideInInspector] public Vector2Int startPosition;

    private void Start()
    {
        cellTypes = new ECellType[gridWidth, gridHeight];


        startPosition = new Vector2Int(gridWidth - 1, gridHeight - 1);
        Item startCellItem = new Item(startCellData, 1);
        grid[startPosition.x, startPosition.y] = startCellItem;
        cellTypes[startPosition.x, startPosition.y] = ECellType.Start;
        SkillInventoryLoader skillInventoryLoader = new SkillInventoryLoader(layout, this);
        EventBus.OnChangeInventory?.Invoke();
    }

    protected override bool CanPlaceItem(Vector2Int position, Item item)
    {
        if (!base.CanPlaceItem(position, item))
            return false;

        foreach (var cell in item.Shape)
        {
            Vector2Int targetPos = position + cell;
            if (!IsInsideGrid(targetPos)) return false;

            ECellType cellType = cellTypes[targetPos.x, targetPos.y];
            switch (cellType)
            {
                case ECellType.Blocked:
                    return false;
                case ECellType.ConditionalBlocked:
                    if (!IsSkillUnlocked(targetPos))
                    {
                        return false;
                    }
                    break;
            }
        }

        HashSet<Vector2Int> occupied = GetTempOccupied();
        foreach (var cell in item.Shape)
        {
            occupied.Add(position + cell);
        }

        return IsConnectedToStart(occupied);
    }

    private bool IsSkillUnlocked(Vector2Int targetPos)
    {
        CellData cellData = GetCellDataAt(targetPos);
        int reqPosX = cellData.prerequisites.x;
        int reqPosY = cellData.prerequisites.y;
        return grid[reqPosX, reqPosY] == null ? false : true;
    }

    private bool IsConnectedToStart(HashSet<Vector2Int> occupied)
    {
        Vector2Int startPos = new Vector2Int(gridWidth - 1, gridHeight - 1);
        if (!occupied.Contains(startPos)) return false;

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startPos);
        visited.Add(startPos);

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (var dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (occupied.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return visited.Count == occupied.Count;
    }


    private bool IsInsideGrid(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridWidth && position.y >= 0 && position.y < gridHeight;
    }

    public override bool RemoveItemAt(Vector2Int position, bool isReplace = false)
    {
        Item targetItem = GetItemAt(position);
        if (targetItem == null) return false;

        HashSet<Vector2Int> occupied = GetTempOccupied();

        foreach (var cell in targetItem.Shape)
        {
            Vector2Int itemPos = position + cell;
            if (IsInsideGrid(itemPos)) occupied.Remove(itemPos);
        }

        if (!IsConnectedToStart(occupied)) return false;
        if (!CheckCondition(targetItem, position)) return false;

        foreach (var cell in targetItem.Shape)
        {
            Vector2Int targetPos = position + cell;
            if (IsInsideGrid(targetPos))
            {
                ECellType cellType = cellTypes[targetPos.x, targetPos.y];

                if (cellType == ECellType.PassiveUnlock || cellType == ECellType.ActiveUnlock)
                {
                    SkillManager.Instance.LockSkill(GetSkillDataAt(targetPos));
                }

                grid[targetPos.x, targetPos.y] = null;
            }
        }
        itemList.Remove(targetItem);
        if (!isReplace) EventBus.OnChangeInventory?.Invoke();

        return true;
    }

    private bool CheckCondition(Item targetItem, Vector2Int position)
    {
        foreach (var cell in targetItem.Shape)
        {
            Vector2Int targetPos = position + cell;
            ECellType cellType = cellTypes[targetPos.x, targetPos.y];

            if (cellType == ECellType.ActiveUnlock)
            {
                CellData cellData = GetCellDataAt(targetPos);
                if (cellData != null)
                {
                    foreach (var conditionPos in cellData.requiredEmptyPositions)
                    {
                        if (GetItemAt(conditionPos) != null)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private HashSet<Vector2Int> GetTempOccupied()
    {
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] != null)
                    occupied.Add(new Vector2Int(x, y));
            }
        }

        return occupied;
    }

    public override bool PlaceItem(Vector2Int position, Item item)
    {
        if (!CanPlaceItem(position, item)) return false;

        if (!base.PlaceItem(position, item)) return false;

        foreach (var cell in item.Shape)
        {
            Vector2Int targetPos = position + cell;
            ECellType type = cellTypes[targetPos.x, targetPos.y];

            if (type == ECellType.PassiveUnlock || type == ECellType.ActiveUnlock)
            {
                SkillManager.Instance.UnlockSkill(GetSkillDataAt(targetPos));
            }
        }

        return true;
    }

    public void SetCellType(Vector2Int pos, ECellType type)
    {
        cellTypes[pos.x, pos.y] = type;
    }

    public void RegisterCell(Vector2Int pos, CellData cell)
    {
        CellDataMap[pos] = cell;
        cellTypes[pos.x, pos.y] = cell.cellType;
    }

    public CellData GetCellDataAt(Vector2Int pos)
    {
        CellDataMap.TryGetValue(pos, out var cell);
        return cell;
    }

    public SkillData GetSkillDataAt(Vector2Int pos)
    {
        return GetCellDataAt(pos).skill;
    }
}