using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillInventoryEditorWindow : EditorWindow
{
    private SkillInventoryLayout layout;
    private Vector2 scrollPos;

    private const int cellSize = 40;
    private CellData selectedCell;

    [MenuItem("Tools/Skill Tree Editor")]
    public static void ShowWindow()
    {
        GetWindow<SkillInventoryEditorWindow>("Skill Tree Editor");
    }

    private void OnGUI()
    {
        layout = (SkillInventoryLayout)EditorGUILayout.ObjectField("Layout", layout, typeof(SkillInventoryLayout), false);
        if (layout == null) return;

        EditorGUILayout.BeginHorizontal();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(1000));
        DrawGrid();
        EditorGUILayout.EndScrollView();

        DrawCellEditor();

        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
            EditorUtility.SetDirty(layout);
    }

    private void DrawGrid()
    {
        if (layout.cells == null) return;

        for (int y = 0; y < layout.gridHeight; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < layout.gridWidth; x++)
            {
                var pos = new Vector2Int(x, y);
                var cell = layout.cells.Find(c => c.position == pos);

                ECellType type = cell != null ? cell.cellType : ECellType.None;

                Color color = GetColorForCell(type);
                GUI.backgroundColor = color;

                if (GUILayout.Button(type.ToString(), GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                {
                    if (cell == null)
                    {
                        cell = new CellData { position = pos, cellType = ECellType.None };
                        layout.cells.Add(cell);
                    }

                    selectedCell = cell;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        GUI.backgroundColor = Color.white;
    }

    private void DrawCellEditor()
    {
        if (selectedCell == null) return;

        EditorGUILayout.BeginVertical("box", GUILayout.Width(300));
        EditorGUILayout.LabelField("Selected Cell", EditorStyles.boldLabel);

        selectedCell.position = EditorGUILayout.Vector2IntField("Position", selectedCell.position);
        selectedCell.cellType = (ECellType)EditorGUILayout.EnumPopup("Cell Type", selectedCell.cellType);

        if (selectedCell.cellType == ECellType.ActiveUnlock || selectedCell.cellType == ECellType.PassiveUnlock)
        {
            selectedCell.skill = (SkillData)EditorGUILayout.ObjectField("Skill", selectedCell.skill, typeof(SkillData), false);
        }

        if (selectedCell.cellType == ECellType.ActiveUnlock)
        {
            if (selectedCell.requiredEmptyPositions == null)
                selectedCell.requiredEmptyPositions = new List<Vector2Int>();

            EditorGUILayout.LabelField("Change Conditions");
            for (int j = 0; j < selectedCell.requiredEmptyPositions.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                selectedCell.requiredEmptyPositions[j] = EditorGUILayout.Vector2IntField($"Condition {j + 1}", selectedCell.requiredEmptyPositions[j]);

                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    selectedCell.requiredEmptyPositions.RemoveAt(j);
                    EditorUtility.SetDirty(layout);
                    Repaint();
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Condition"))
            {
                selectedCell.requiredEmptyPositions.Add(Vector2Int.zero);
                EditorUtility.SetDirty(layout);
                Repaint();
            }
        }

        if (selectedCell.cellType == ECellType.ConditionalBlocked)
        {
            selectedCell.prerequisites = EditorGUILayout.Vector2IntField("Prerequisite", selectedCell.prerequisites);
        }

        if (GUILayout.Button("Clear Selection"))
        {
            selectedCell = null;
        }

        EditorGUILayout.EndVertical();
    }




    private Color GetColorForCell(ECellType type)
    {
        switch (type)
        {
            case ECellType.Blocked:
                return Color.black;
            case ECellType.ConditionalBlocked:
                return Color.gray;
            case ECellType.ActiveUnlock:
                return Color.red;
            case ECellType.PassiveUnlock:
                return Color.green;
            case ECellType.Start:
                return Color.cyan;
            default:
                return Color.white;
        }
    }
}
