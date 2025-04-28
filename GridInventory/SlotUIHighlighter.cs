using System.Collections.Generic;
using UnityEngine;

public class SlotUIHighlighter : MonoBehaviour
{
    [SerializeField] private GameObject highlighterPrefab;
    private List<RectTransform> activeHighlighters = new List<RectTransform>();
    private Queue<RectTransform> highlightPool = new Queue<RectTransform>();

    /// <summary>
    /// 모든 하이라이터 제거 (풀로 반환)
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
    /// 아이템이 이미 배치된 상태에서 하이라이터 표시
    /// </summary>
    public void SetPosition(ItemGrid grid, Item item)
    {
        Clear();
        ShowHighlightCells(grid, item, item.posX, item.posY);
    }

    /// <summary>
    /// 드래그 중인 아이템을 특정 좌표에 놓을 때 미리보기 하이라이터 표시
    /// </summary>
    public void SetPosition(ItemGrid grid, Item item, int x, int y)
    {
        Clear();
        ShowHighlightCells(grid, item, x, y);
    }

    /// <summary>
    /// 하이라이터 셀 단위로 출력
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
    /// 풀에서 꺼내거나 새로 생성
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
    /// 현재 활성화된 하이라이터를 전체 보이기/숨기기
    /// </summary>
    public void Show(bool flag)
    {
        foreach (var highlight in activeHighlighters)
        {
            highlight.gameObject.SetActive(flag);
        }
    }
}
