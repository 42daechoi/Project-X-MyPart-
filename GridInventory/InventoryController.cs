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
            // 아이템(이름)을 판매하시겠습니까? 예 아니오 버튼 UI
            Inventory.Instance.RemoveItemAt(tileGridPosition, false);
        }
    }

    /// <summary>
    /// 아이템 드래그를 시작합니다. 드래그 중이 아니고 아이템이 있다면 선택합니다.
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
    /// 드래그 중일 때 아이템의 위치를 마우스를 따라 이동
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
    /// 드래그를 끝내고 인벤토리 안이면 배치하고, 아니면 아이템을 버리기
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
    /// 아이템을 인벤토리 밖으로 드롭
    /// </summary>
    private void DropItem()
    {
        Inventory.Instance.DropItem(pickUpItem);
        pickUpItem = null;
        pickUpItemTransform = null;
        rotationCommands.Clear();
    }

    /// <summary>
    /// 현재 마우스 포인터가 인벤토리 패널 위에 있는지 확인
    /// </summary>
    private bool IsPointerOverInventoryPanel()
    {
        Vector2 localMousePosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryPanelTransform, Input.mousePosition, null, out localMousePosition);
        return inventoryPanelTransform.rect.Contains(localMousePosition);
    }

    /// <summary>
    /// 마우스 위치를 그리드 좌표로 변환
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
    /// 현재 선택된 아이템을 회전
    /// </summary>
    private void RotateItem()
    {
        if (pickUpItem == null) return;

        RotateCommand command = new RotateCommand(pickUpItem);
        command.Execute();
        rotationCommands.Push(command);
    }

    /// <summary>
    /// 아이템 회전을 모두 취소합니다 (되돌리기)
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
    /// 아이템을 지정된 위치에 배치 시도. 실패하면 되돌리거나 자동 재배치
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
    /// 지정된 그리드 위치에서 아이템을 픽업
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
    /// 마우스 아래의 슬롯에 하이라이트를 표시
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
    /// 드래그 중인 아이템 아이콘을 마우스 위치로 이동
    /// </summary>
    private void ItemIconDrag()
    {
        if (pickUpItemTransform != null)
        {
            pickUpItemTransform.position = Input.mousePosition;
        }
    }
}
