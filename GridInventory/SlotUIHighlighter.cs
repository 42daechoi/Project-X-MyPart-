using System.Collections.Generic;
using UnityEngine;

public class SlotUIHighlighter : MonoBehaviour
{
    [SerializeField] private GameObject highlighterPrefab;
    private List<RectTransform> activeHighlighters = new List<RectTransform>();
    private Queue<RectTransform> highlightPool = new Queue<RectTransform>();

    /// <summary>
    /// ��� ���̶����� ���� (Ǯ�� ��ȯ)
    /// </summary>
    public void Clear()
    {
        foreach (var highlight in activeHighlighters)
        {
            highlight.gameObject.SetActive(false);
            highlightPool.Enqueue(highlight);
        }
        activeHighlighters.Clear();
    }

    /// <summary>
    /// �������� �̹� ��ġ�� ���¿��� ���̶����� ǥ��
    /// </summary>
    public void SetPosition(ItemGrid grid, Item item)
    {
        Clear();
        ShowHighlightCells(grid, item, item.posX, item.posY);
    }

    /// <summary>
    /// �巡�� ���� �������� Ư�� ��ǥ�� ���� �� �̸����� ���̶����� ǥ��
    /// </summary>
    public void SetPosition(ItemGrid grid, Item item, int x, int y)
    {
        Clear();
        ShowHighlightCells(grid, item, x, y);
    }

    /// <summary>
    /// ���̶����� �� ������ ���
    /// </summary>
    private void ShowHighlightCells(ItemGrid grid, Item item, int posX, int posY)
    {
        foreach (var cell in item.Shape)
        {
            int cellX = posX + cell.x;
            int cellY = posY + cell.y;
            Vector2 localPos = grid.CalculateCellPosition(cellX, cellY);

            RectTransform rect = GetHighlighter();
            rect.SetParent(grid.transform, false);
            rect.sizeDelta = new Vector2(ItemGrid.tileSizeWidth, ItemGrid.tileSizeHeight);
            rect.localPosition = localPos;
            rect.gameObject.SetActive(true);

            activeHighlighters.Add(rect);
        }
    }

    /// <summary>
    /// Ǯ���� �����ų� ���� ����
    /// </summary>
    private RectTransform GetHighlighter()
    {
        if (highlightPool.Count > 0)
        {
            return highlightPool.Dequeue();
        }

        GameObject go = Instantiate(highlighterPrefab);
        return go.GetComponent<RectTransform>();
    }

    /// <summary>
    /// ���� Ȱ��ȭ�� ���̶����͸� ��ü ���̱�/�����
    /// </summary>
    public void Show(bool flag)
    {
        foreach (var highlight in activeHighlighters)
        {
            highlight.gameObject.SetActive(flag);
        }
    }
}
