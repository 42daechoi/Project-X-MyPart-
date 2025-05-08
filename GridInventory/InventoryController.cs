using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [HideInInspector] public ItemGrid selectedItemGrid;
    [HideInInspector] private ItemGrid prevItemGrid;
    [HideInInspector] private SlotUIHighlighter slotUIHighlighter;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private RectTransform pickUpItemTransform;
    [SerializeField] private RectTransform inventoryPanelTransform;
    [SerializeField] private RectTransform dragLayer;
     private Item pickUpItem;
    [SerializeField] private Item itemToHighlight;

    [SerializeField] private bool canSell = false;
    private bool isDragging = false;
    private Stack<ICommand> rotationCommands = new Stack<ICommand>();


    private void Start()
    {
        pickUpItem = null;
        slotUIHighlighter = GetComponent<SlotUIHighlighter>();
        graphicRaycaster = GetComponentInChildren<GraphicRaycaster>();
        EventBus.OnInteractMerchant += ChangeCanSell;
    }

    private void OnDisable()
    {
        EventBus.OnInteractMerchant -= ChangeCanSell;
    }


    private void Update()
    {
        selectedItemGrid = GetGridUnderMouse();
        ItemIconDrag();
        if (selectedItemGrid == null)
        {
            slotUIHighlighter.Show(false);
        }
        if (Input.GetMouseButtonDown(1) && canSell)
        {
            SellItem();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }

        if (isDragging)
        {
            DragItem();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void ChangeCanSell(string merchantType, bool isInteract)
    {
        canSell = isInteract;
    }

    private ItemGrid GetGridUnderMouse()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            ItemGrid grid = result.gameObject.GetComponentInParent<ItemGrid>();
            if (grid != null && grid.gameObject.activeSelf)
                return grid;
        }

        return null;
    }

    private void SellItem()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();
        Item item = selectedItemGrid.inventory.GetItemAt(tileGridPosition);
        if (item != null)
        {
            // ������(�̸�)�� �Ǹ��Ͻðڽ��ϱ�? �� �ƴϿ� ��ư UI
            selectedItemGrid.inventory.RemoveItemAt(tileGridPosition, false);
        }
    }

    /// <summary>
    /// ������ �巡�׸� �����մϴ�. �巡�� ���� �ƴϰ� �������� �ִٸ� �����մϴ�.
    /// </summary>
    private void StartDrag()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();
        if (tileGridPosition.x == -1 && tileGridPosition.y == -1) return;
        if (pickUpItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            isDragging = true;
        }
    }

    /// <summary>
    /// �巡�� ���� �� �������� ��ġ�� ���콺�� ���� �̵�
    /// </summary>
    private void DragItem()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        if (pickUpItem != null)
        {
            slotUIHighlighter.Show(true);
            slotUIHighlighter.SetPosition(selectedItemGrid, pickUpItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    /// <summary>
    /// �巡�׸� ������ �κ��丮 ���̸� ��ġ�ϰ�, �ƴϸ� �������� ������
    /// </summary>
    private void EndDrag()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();
        if (pickUpItem != null)
        {
            //if (IsPointerOverInventoryPanel())
            //{
                PlaceItem(tileGridPosition);
            //}
            //else
            //{
            //    DropItem();
            //}
        }
        isDragging = false;
    }

    /// <summary>
    /// �������� �κ��丮 ������ ���
    /// </summary>
    private void DropItem()
    {
        selectedItemGrid.inventory.DropItem(pickUpItem);
        pickUpItem = null;
        pickUpItemTransform = null;
        rotationCommands.Clear();
    }

    /// <summary>
    /// ���� ���콺 �����Ͱ� �κ��丮 �г� ���� �ִ��� Ȯ��
    /// </summary>
    //private bool IsPointerOverInventoryPanel()
    //{
    //    Vector2 localMousePosition = Vector2.zero;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryPanelTransform, Input.mousePosition, null, out localMousePosition);
    //    return inventoryPanelTransform.rect.Contains(localMousePosition);
    //}

    /// <summary>
    /// ���콺 ��ġ�� �׸��� ��ǥ�� ��ȯ
    /// </summary>
    private Vector2Int GetTileGridPosition()
    {
        if (selectedItemGrid == null) return new Vector2Int(-1, -1);

        Vector2 position = Input.mousePosition;

        if (pickUpItem != null)
        {
            position.x -= (pickUpItem.BoundingSize.x - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (pickUpItem.BoundingSize.y - 1) * ItemGrid.tileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    /// <summary>
    /// ���� ���õ� �������� ȸ��
    /// </summary>
    private void RotateItem()
    {
        if (pickUpItem == null) return;

        RotateCommand command = new RotateCommand(pickUpItem);
        command.Execute();
        rotationCommands.Push(command);
    }

    /// <summary>
    /// ������ ȸ���� ��� ����մϴ� (�ǵ�����)
    /// </summary>
    private void UndoAllRotationCommands()
    {
        while (rotationCommands.Count > 0)
        {
            var command = rotationCommands.Pop();
            command.Undo();
        }
    }

    /// <summary>
    /// �������� ������ ��ġ�� ��ġ �õ�. �����ϸ� �ǵ����ų� �ڵ� ���ġ
    /// </summary>
    private void PlaceItem(Vector2Int tileGridPosition)
    {
        pickUpItemTransform.SetParent(prevItemGrid.transform, false);
        Vector2Int prevPosition = new Vector2Int(pickUpItem.posX, pickUpItem.posY);
        if (selectedItemGrid == null || !selectedItemGrid.inventory.PlaceItem(tileGridPosition, pickUpItem))
        {
            UndoAllRotationCommands();
            if (!prevItemGrid.inventory.PlaceItem(prevPosition, pickUpItem))
            {
                if (prevItemGrid.inventoryType == EInventoryType.Item)
                {
                    prevItemGrid.inventory.TryAddItem(pickUpItem);
                }
            }
        }

        EnableRaycastForPickUpIcon();
        prevItemGrid = null;
        pickUpItem = null;
        pickUpItemTransform = null;
        rotationCommands.Clear();
    }

    /// <summary>
    /// ������ �׸��� ��ġ���� �������� �Ⱦ�
    /// </summary>
    private void PickUpItem(Vector2Int tileGridPosition)
    {
        if (selectedItemGrid.inventory is SkillTreeInventory skillTreeInventory)
        {
            if (tileGridPosition == skillTreeInventory.startPosition)
            {
                return;
            }
        }
        pickUpItem = selectedItemGrid.inventory.GetItemAt(tileGridPosition);
        if (pickUpItem != null)
        {
            if (selectedItemGrid.inventory.RemoveItemAt(tileGridPosition, true))
            {
                pickUpItemTransform = pickUpItem.GetSlotUI().GetComponent<RectTransform>();
                Vector3 worldPos = pickUpItemTransform.position;
                pickUpItemTransform.SetParent(dragLayer, false);
                pickUpItemTransform.position = worldPos;
                DisableRaycastForPickUpIcon();

                prevItemGrid = selectedItemGrid;
            }
            else
            {
                pickUpItem = null;
            }
        }
    }

    /// <summary>
    /// ���콺 �Ʒ��� ���Կ� ���̶���Ʈ�� ǥ��
    /// </summary>
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        if (positionOnGrid.x == -1 && positionOnGrid.y == -1) return;
        if (pickUpItem == null)
        {
            itemToHighlight = selectedItemGrid.inventory.GetItemAt(positionOnGrid);
            if (itemToHighlight != null)
            {
                slotUIHighlighter.Show(true);
                slotUIHighlighter.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                slotUIHighlighter.Show(false);
            }
        }
        else
        {
            slotUIHighlighter.Show(true);
            slotUIHighlighter.SetPosition(selectedItemGrid, pickUpItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    /// <summary>
    /// �巡�� ���� ������ �������� ���콺 ��ġ�� �̵�
    /// </summary>
    private void ItemIconDrag()
    {
        if (pickUpItemTransform != null)
        {
            pickUpItemTransform.position = Input.mousePosition;
        }
    }

    private void DisableRaycastForPickUpIcon()
    {
        if (pickUpItemTransform != null)
        {
            var image = pickUpItemTransform.GetComponent<Image>();
            if (image != null) image.raycastTarget = false;
        }
    }


    private void EnableRaycastForPickUpIcon()
    {
        if (pickUpItemTransform != null)
        {
            var image = pickUpItemTransform.GetComponent<Image>();
            if (image != null) image.raycastTarget = true;
        }
    }
}
