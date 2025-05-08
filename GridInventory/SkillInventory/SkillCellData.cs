using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellData
{
    public Vector2Int position;
    public ECellType cellType;
    public SkillData skill;
    public Vector2Int prerequisites;
    public List<Vector2Int> requiredEmptyPositions;
}