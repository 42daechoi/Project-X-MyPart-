using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public ItemData data;
    public int posX;
    public int posY;
    public int currentStack;
    public int rotationIndex;
    private List<Vector2Int> rotatedShape;
    private Vector2Int boundingSize;
    private InventorySlotUI slotUI;


    public int RotationIndex => rotationIndex;
    public bool IsFull => currentStack >= data.maxStackSize;
    public List<Vector2Int> Shape => rotatedShape;
    public Vector2Int BoundingSize => boundingSize;

    public Item(ItemData data, int stack)
    {
        this.data = data;
        this.rotationIndex = 0;
        this.currentStack = stack;
        UpdateRotatedShape();
    }
    public Vector2Int GetDefaultBoundSize()
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (var cell in data.shape)
        {
            if (cell.x < minX) minX = cell.x;
            if (cell.y < minY) minY = cell.y;
            if (cell.x > maxX) maxX = cell.x;
            if (cell.y > maxY) maxY = cell.y;
        }

        int width = (maxX - minX) + 1;
        int height = (maxY - minY) + 1;
        return new Vector2Int(width, height);
    }

    private void UpdateRotatedShape()
    {
        rotatedShape = new List<Vector2Int>();

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (var cell in data.shape)
        {
            Vector2Int rotated = RotateCell(cell, rotationIndex);
            rotatedShape.Add(rotated);

            if (rotated.x < minX) minX = rotated.x;
            if (rotated.y < minY) minY = rotated.y;
            if (rotated.x > maxX) maxX = rotated.x;
            if (rotated.y > maxY) maxY = rotated.y;
        }

        for (int i = 0; i < rotatedShape.Count; i++)
        {
            rotatedShape[i] -= new Vector2Int(minX, minY);
        }

        int width = (maxX - minX) + 1;
        int height = (maxY - minY) + 1;
        boundingSize = new Vector2Int(width, height);
    }

    public void SetSlotUI(InventorySlotUI slotUI) => this.slotUI = slotUI;
    public InventorySlotUI GetSlotUI() => slotUI;

    public void Rotate()
    {
        rotationIndex = (rotationIndex + 1) % 4;
        UpdateRotatedShape();
        slotUI?.UpdateRotation();
    }

    private Vector2Int RotateCell(Vector2Int cell, int index)
    {
        switch (index % 4)
        {
            case 0: return new Vector2Int(cell.x, cell.y);
            case 1: return new Vector2Int(-cell.y, cell.x);
            case 2: return new Vector2Int(-cell.x, -cell.y);
            case 3: return new Vector2Int(cell.y, -cell.x);
            default: return cell;
        }
    }

    public bool CanStackWith(Item other)
    {
        return (data == other.data && data.isStackable && !IsFull);
    }

    public int AddToStack(int amount)
    {
        int space = data.maxStackSize - currentStack;
        int added = Mathf.Min(space, amount);
        currentStack += added;

        return added;
    }
}
