using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector] public ItemGrid selectedItemGrid;
    [HideInInspector] private SlotUIHighlighter slotUIHighlighter;
    [SerializeField] private RectTransform pickUpItemTransform;
    [SerializeField] private RectTransform inventoryPanelTransform;
    [SerializeField] private Item pickUpItem;
    [SerializeField] private Item itemToHighlight;

    [SerializeField] private bool canSell = false;
    private bool isDragging = false;
    private Stack<ICommand> rotationCommands = new Stack<ICommand>();


    private void Start()
    {
        slotUIHighlighter = GetComponent<SlotUIHighlighter>();
        EventBus.OnInteractMerchant += ChangeCanSell;
    }

    private void OnDisable()
    {
        EventBus.OnInteractMerchant -= ChangeCanSell;
    }

    private void ChangeCanSell(string merchantType, bool isInteract)
    {
        canSell = isInteract;
    }

    private void Update()
    {
        ItemIconDrag();

        if (selectedItemGrid == null)
        {
            slotUIHighlighter.Show(false);
            return;
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

    private void SellItem()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();
        Item item = Inventory.Instance.GetItemAt(tileGridPosition);
        if (item != null)
        {
            // ������(�̸�)�� �Ǹ��Ͻðڽ��ϱ�? �� �ƴϿ� ��ư UI
            Inventory.Instance.RemoveItemAt(tileGridPosition, false);
        }
    }

    /// <summary>
    /// ������ �巡�׸� �����մϴ�. �巡�� ���� �ƴϰ� �������� �ִٸ� �����մϴ�.
    /// </summary>
    private void StartDrag()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();

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
            if (IsPointerOverInventoryPanel())
            {
                PlaceItem(tileGridPosition);
            }
            else
            {
                DropItem();
            }
        }
        isDragging = false;
    }

    /// <summary>
    /// �������� �κ��丮 ������ ���
    /// </summary>
    private void DropItem()
    {
        Inventory.Instance.DropItem(pickUpItem);
        pickUpItem = null;
        pickUpItemTransform = null;
        rotationCommands.Clear();
    }

    /// <summary>
    /// ���� ���콺 �����Ͱ� �κ��丮 �г� ���� �ִ��� Ȯ��
    /// </summary>
    private bool IsPointerOverInventoryPanel()
    {
        Vector2 localMousePosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryPanelTransform, Input.mousePosition, null, out localMousePosition);
        return inventoryPanelTransform.rect.Contains(localMousePosition);
    }

    /// <summary>
    /// ���콺 ��ġ�� �׸��� ��ǥ�� ��ȯ
    /// </summary>
    private Vector2Int GetTileGridPosition()
    {
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
        Vector2Int prevPosition = new Vector2Int(pickUpItem.posX, pickUpItem.posY);

        if (!Inventory.Instance.PlaceItem(tileGridPosition, pickUpItem))
        {
            UndoAllRotationCommands();

            if (!Inventory.Instance.PlaceItem(prevPosition, pickUpItem))
            {
                Inventory.Instance.TryAddItem(pickUpItem);
            }
        }

        pickUpItem = null;
        pickUpItemTransform = null;
        rotationCommands.Clear();
    }

    /// <summary>
    /// ������ �׸��� ��ġ���� �������� �Ⱦ�
    /// </summary>
    private void PickUpItem(Vector2Int tileGridPosition)
    {
        pickUpItem = Inventory.Instance.GetItemAt(tileGridPosition);
        if (pickUpItem != null)
        {
            pickUpItemTransform = pickUpItem.GetSlotUI().GetComponent<RectTransform>();
            Inventory.Instance.RemoveItemAt(tileGridPosition, true);
        }
    }

    /// <summary>
    /// ���콺 �Ʒ��� ���Կ� ���̶���Ʈ�� ǥ��
    /// </summary>
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        if (pickUpItem == null)
        {
            itemToHighlight = Inventory.Instance.GetItemAt(positionOnGrid);
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
}
