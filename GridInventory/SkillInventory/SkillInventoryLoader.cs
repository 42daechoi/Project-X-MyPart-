using UnityEngine;

public class SkillInventoryLoader
{
    public SkillInventoryLayout layout;
    public SkillTreeInventory inventory;


    public SkillInventoryLoader(SkillInventoryLayout layout, SkillTreeInventory inventory)
    {
        this.layout = layout;
        this.inventory = inventory;

        if (layout == null) return;

        foreach (var cell in layout.cells)
        {
            PlaceCell(cell);
        }
    }

    private void PlaceCell(CellData cell)
    {
        if (IsValidGridPosition(cell.position))
        {
            inventory.RegisterCell(cell.position, cell);
        }
    }

    private bool IsValidGridPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < inventory.GetGridWidth() && pos.y >= 0 && pos.y < inventory.GetGridHeight();
    }

}
