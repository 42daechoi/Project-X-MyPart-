using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillInventoryLayout", menuName = "Inventory/SkillInventoryLayout")]
public class SkillInventoryLayout : ScriptableObject
{
    public int gridWidth;
    public int gridHeight;
    public List<CellData> cells;
}