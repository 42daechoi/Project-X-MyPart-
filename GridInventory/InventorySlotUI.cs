using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI stackCountText;

    public void SetItem(Item item, Canvas rootCanvas)
    {
        this.item = item;
        SetIcon(item.data.icon, rootCanvas);
        SetStackCount(item.currentStack);
        item.SetSlotUI(this);
        UpdateRotation();
    }

    public void SetIcon(Sprite icon, Canvas rootCanvas)
    {
        iconImage.enabled = true;
        iconImage.sprite = icon;

        Vector2 size = new Vector2();
        Vector2Int boundingSize = item.GetDefaultBoundSize();
        size.x = boundingSize.x * ItemGrid.tileSizeWidth;
        size.y = boundingSize.y * ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void SetStackCount(int stackCount)
    {
        stackCountText.text = stackCount > 1 ? stackCount.ToString() : "";
    }

    public void UpdateRotation()
    {
        if (item != null)
        {
            GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, item.RotationIndex * 90f);
        }
    }

}
