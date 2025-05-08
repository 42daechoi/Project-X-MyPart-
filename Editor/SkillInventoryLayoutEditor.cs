using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillInventoryLayout))]
public class SkillInventoryLayoutEditor : Editor
{
    private SkillInventoryLayout layout;

    private void OnEnable()
    {
        layout = (SkillInventoryLayout)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Grid"))
        {
            GenerateGrid();
        }

        layout.gridWidth = EditorGUILayout.IntField("Grid Width", layout.gridWidth);
        layout.gridHeight = EditorGUILayout.IntField("Grid Height", layout.gridHeight);

        for (int i = 0; i < layout.cells.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            var cell = layout.cells[i];
            cell.position = EditorGUILayout.Vector2IntField("Position", cell.position);
            cell.cellType = (ECellType)EditorGUILayout.EnumPopup("Cell Type", cell.cellType);
            cell.skill = (SkillData)EditorGUILayout.ObjectField("Skill", cell.skill, typeof(SkillData), false);

            if (cell.cellType == ECellType.ActiveUnlock)
            {
                if (cell.requiredEmptyPositions == null)
                    cell.requiredEmptyPositions = new List<Vector2Int>();

                EditorGUILayout.LabelField("Change Conditions");
                for (int j = 0; j < cell.requiredEmptyPositions.Count; j++)
                {
                    cell.requiredEmptyPositions[j] = EditorGUILayout.Vector2IntField($"Condition {j + 1}", cell.requiredEmptyPositions[j]);
                }
                if (GUILayout.Button("Add Condition"))
                {
                    cell.requiredEmptyPositions.Add(Vector2Int.zero);
                }
            }

            if (cell.cellType == ECellType.ConditionalBlocked)
            {
                cell.prerequisites = EditorGUILayout.Vector2IntField("Prerequisite Position", cell.prerequisites);
            }

            if (GUILayout.Button("Remove"))
            {
                layout.cells.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Cell"))
        {
            AddCell();
        }

        serializedObject.ApplyModifiedProperties();
    }


    private void AddCell()
    {
        layout.cells.Add(new CellData());
    }

    private void GenerateGrid()
    {
        layout.cells.Clear();
        for (int y = 0; y < layout.gridHeight; y++)
        {
            for (int x = 0; x < layout.gridWidth; x++)
            {
                layout.cells.Add(new CellData
                {
                    position = new Vector2Int(x, y),
                    cellType = ECellType.None,
                    skill = null,
                    prerequisites = new Vector2Int(-1, -1)
                });
            }
        }
    }
}
