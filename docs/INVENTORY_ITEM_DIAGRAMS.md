# 📦 인벤토리 & 아이템 시스템 다이어그램

## 목차
1. [인벤토리 시스템 클래스 다이어그램](#1-인벤토리-시스템-클래스-다이어그램)
2. [아이템 시스템 클래스 다이어그램](#2-아이템-시스템-클래스-다이어그램)
3. [스탯 시스템 클래스 다이어그램](#3-스탯-시스템-클래스-다이어그램)
4. [아이템 드래그 앤 드롭 시퀀스](#4-아이템-드래그-앤-드롭-시퀀스)
5. [아이템 회전 시퀀스](#5-아이템-회전-시퀀스)
6. [아이템 자동 배치 시퀀스](#6-아이템-자동-배치-시퀀스)
7. [인벤토리 UI 업데이트 시퀀스](#7-인벤토리-ui-업데이트-시퀀스)

---

## 1. 인벤토리 시스템 클래스 다이어그램

그리드 기반 인벤토리 시스템의 핵심 클래스 구조입니다. `Inventory` 클래스를 기반으로 `SkillTreeInventory`가 상속받아 스킬 해금 로직을 추가합니다.

```mermaid
classDiagram
    direction TB
    
    class Inventory {
        <<MonoBehaviour>>
        #int gridWidth
        #int gridHeight
        +EInventoryType inventoryType
        #Item[,] grid
        #List~Item~ itemList
        +Init()
        #CanPlaceItem(Vector2Int, Item) bool
        +PlaceItem(Vector2Int, Item) bool
        +RemoveItemAt(Vector2Int, bool) bool
        +DropItem(Item)
        +TryAddItem(Item) bool
        +GetItemAt(Vector2Int) Item
        +GetTotalGearStats() Stats
        +ExpandInventory()
        +DecreaseItemStack(string) bool
    }
    
    class SkillTreeInventory {
        <<MonoBehaviour>>
        +SkillInventoryLayout layout
        +ItemData startCellData
        -ECellType[,] cellTypes
        -Dictionary~Vector2Int,CellData~ CellDataMap
        +Vector2Int startPosition
        #CanPlaceItem(Vector2Int, Item) bool
        +PlaceItem(Vector2Int, Item) bool
        +RemoveItemAt(Vector2Int, bool) bool
        -IsSkillUnlocked(Vector2Int) bool
        -IsConnectedToStart(HashSet~Vector2Int~) bool
        +SetCellType(Vector2Int, ECellType)
        +RegisterCell(Vector2Int, CellData)
        +GetCellDataAt(Vector2Int) CellData
        +GetSkillDataAt(Vector2Int) SkillData
    }
    
    class InventoryManager {
        <<Singleton>>
        +Inventory inventory
        +SkillTreeInventory skillTreeInventory
        -GameObject inventoryPanel
        -GameObject skillInventoryPanel
        -ItemGrid skillItemGrid
        -ItemGrid itemGrid
        -InitInventories()
        -InitGrid()
        -ToggleInventory()
        -ToggleSkillInventory()
        -ActiveInventory(string, bool)
    }
    
    class InventoryController {
        <<MonoBehaviour>>
        +ItemGrid selectedItemGrid
        -ItemGrid prevItemGrid
        -SlotUIHighlighter slotUIHighlighter
        -Item pickUpItem
        -bool isDragging
        -Stack~ICommand~ rotationCommands
        -Update()
        -StartDrag()
        -DragItem()
        -EndDrag()
        -PickUpItem(Vector2Int)
        -PlaceItem(Vector2Int)
        -RotateItem()
        -UndoAllRotationCommands()
        -SellItem()
    }
    
    class ItemGrid {
        <<MonoBehaviour>>
        +float tileSizeWidth$
        +float tileSizeHeight$
        +Inventory inventory
        +EInventoryType inventoryType
        -Queue~GameObject~ itemObjectPool
        -List~GameObject~ activeItemObjectList
        +Init()
        +GetTileGridPosition(Vector2) Vector2Int
        +UpdateGrid()
        +CalculatePositionOnGrid(Item) Vector2
        +CalculateCellPosition(int, int) Vector2
    }
    
    class ICommand {
        <<interface>>
        +Execute()
        +Undo()
    }
    
    class RotateCommand {
        -Item item
        +RotateCommand(Item)
        +Execute()
        +Undo()
    }
    
    class SlotUIHighlighter {
        <<MonoBehaviour>>
        -List~RectTransform~ activeHighlighters
        -Queue~RectTransform~ highlightPool
        +Clear()
        +SetPosition(ItemGrid, Item)
        +SetPosition(ItemGrid, Item, int, int)
        +Show(bool)
    }
    
    class InventorySlotUI {
        <<MonoBehaviour>>
        -Item item
        -Image iconImage
        -TextMeshProUGUI stackCountText
        +SetItem(Item, Canvas)
        +SetIcon(Sprite, Canvas)
        +SetStackCount(int)
        +UpdateRotation()
    }
    
    Inventory <|-- SkillTreeInventory
    InventoryManager --> Inventory
    InventoryManager --> SkillTreeInventory
    InventoryController --> ItemGrid
    InventoryController --> SlotUIHighlighter
    InventoryController ..|> ICommand : uses
    ItemGrid --> Inventory
    ItemGrid --> InventorySlotUI
    RotateCommand ..|> ICommand
    RotateCommand --> Item
```

---

## 2. 아이템 시스템 클래스 다이어그램

아이템 데이터 및 인스턴스 구조입니다. ScriptableObject 기반의 `ItemData`에서 다양한 아이템 타입으로 상속됩니다.

```mermaid
classDiagram
    direction TB
    
    class Item {
        +ItemData data
        +int posX
        +int posY
        +int currentStack
        +int rotationIndex
        -List~Vector2Int~ rotatedShape
        -Vector2Int boundingSize
        -InventorySlotUI slotUI
        +Item(ItemData, int)
        +GetDefaultBoundSize() Vector2Int
        -UpdateRotatedShape()
        +Rotate()
        -RotateCell(Vector2Int, int) Vector2Int
        +CanStackWith(Item) bool
        +AddToStack(int) int
        +SetSlotUI(InventorySlotUI)
        +GetSlotUI() InventorySlotUI
    }
    
    class ItemData {
        <<ScriptableObject>>
        +string itemName
        +Sprite icon
        +bool isStackable
        +int maxStackSize
        +List~Vector2Int~ shape
    }
    
    class GearData {
        <<ScriptableObject>>
        -Stats stats
        -ItemRarity rarity
        -string specialAbilityDescription
        +GetStats() Stats
        +GetRarity() ItemRarity
        +GetSpecialAbility() string
    }
    
    class GemData {
        <<ScriptableObject>>
        -GemSize gemSize
        -ElementType elementType
        +GemSize GemSize
    }
    
    class ComsumableItemData {
        <<ScriptableObject>>
        -StatBinder effect
        +GetStatBinder() StatBinder
    }
    
    class FarmableItem {
        <<MonoBehaviour>>
        +ItemData itemData
        +int stackCount
        +Pickup()
    }
    
    class ItemRarity {
        <<enumeration>>
        Common
        Rare
        Epic
        Legendary
    }
    
    class GemSize {
        <<enumeration>>
        Small
        Medium
        Large
    }
    
    class EInventoryType {
        <<enumeration>>
        Item
        SkillTree
    }
    
    ItemData <|-- GearData
    ItemData <|-- GemData
    ItemData <|-- ComsumableItemData
    Item --> ItemData
    FarmableItem --> ItemData
    GearData --> ItemRarity
    GemData --> GemSize
    GemData --> ElementType
```

---

## 3. 스탯 시스템 클래스 다이어그램

캐릭터 스탯 및 아이템/스킬 효과에 사용되는 스탯 구조체입니다.

```mermaid
classDiagram
    direction TB
    
    class Stats {
        <<struct>>
        +float meleePower
        +float maxHealth
        +float curHealth
        +float moveSpeed
        +float curFireGauge
        +float maxFireGauge
        +float fireRecovery
        +float additionalFirePower
        +float curElectricGauge
        +float maxElectricGauge
        +float electricRecovery
        +float additionalElectricPower
        +float curEarthGauge
        +float maxEarthGauge
        +float earthRecovery
        +float additionalEarthPower
        +float miningSpeed
        +int pickAxeLevel
        +Zero$ Stats
        +operator+(Stats, Stats) Stats
        +operator-(Stats, Stats) Stats
        +operator+(Stats, StatBinder) Stats
        +operator-(Stats, StatBinder) Stats
    }
    
    class StatBinder {
        <<struct>>
        +StatType type
        +float value
    }
    
    class StatType {
        <<enumeration>>
        MeleePower
        MaxHealth
        CurHealth
        MoveSpeed
        CurFireGauge
        MaxFireGauge
        FireRecovery
        AdditionalFirePower
        CurElectricGauge
        MaxElectricGauge
        ElectricRecovery
        AdditionalElectricPower
        CurEarthGauge
        MaxEarthGauge
        EarthRecovery
        AdditionalEarthPower
        MiningSpeed
        PickAxeLevel
    }
    
    StatBinder --> StatType
    Stats --> StatBinder : uses
```

---

## 4. 아이템 드래그 앤 드롭 시퀀스

인벤토리에서 아이템을 드래그하여 이동시키는 전체 흐름입니다.

```mermaid
sequenceDiagram
    participant User as 👤 User
    participant IC as InventoryController
    participant IG as ItemGrid
    participant INV as Inventory
    participant UI as InventorySlotUI
    participant HL as SlotUIHighlighter

    User->>IC: 마우스 클릭 (MouseButtonDown)
    IC->>IC: GetGridUnderMouse()
    IC->>IG: GetTileGridPosition(mousePosition)
    IG-->>IC: tileGridPosition
    IC->>INV: GetItemAt(position)
    INV-->>IC: item
    
    alt 아이템이 존재하면
        IC->>INV: RemoveItemAt(position, isReplace=true)
        INV->>INV: grid[x,y] = null
        INV-->>IC: true
        IC->>IC: pickUpItem = item
        IC->>IC: DisableRaycastForPickUpIcon()
    end
    
    loop 드래그 중
        User->>IC: 마우스 이동
        IC->>IC: ItemIconDrag()
        IC->>HL: SetPosition(grid, item, x, y)
        HL->>HL: Clear()
        HL->>HL: ShowHighlightCells()
    end
    
    User->>IC: 마우스 릴리즈 (MouseButtonUp)
    IC->>IG: GetTileGridPosition(mousePosition)
    IG-->>IC: newPosition
    IC->>INV: PlaceItem(newPosition, item)
    INV->>INV: CanPlaceItem(position, item)
    
    alt 배치 가능
        INV->>INV: grid[x,y] = item
        INV->>INV: itemList.Add(item)
        INV->>EventBus: OnChangeInventory?.Invoke()
        INV-->>IC: true
    else 배치 불가능
        IC->>IC: UndoAllRotationCommands()
        IC->>INV: PlaceItem(prevPosition, item)
    end
    
    IC->>IC: EnableRaycastForPickUpIcon()
    IC->>IC: pickUpItem = null
```

---

## 5. 아이템 회전 시퀀스

테트리스 스타일로 아이템을 회전시키는 흐름입니다. Command 패턴을 사용하여 Undo가 가능합니다.

```mermaid
sequenceDiagram
    participant User as 👤 User
    participant IC as InventoryController
    participant RC as RotateCommand
    participant Item as Item
    participant UI as InventorySlotUI

    User->>IC: R키 입력
    IC->>IC: RotateItem()
    
    alt pickUpItem이 존재하면
        IC->>RC: new RotateCommand(pickUpItem)
        IC->>RC: Execute()
        RC->>Item: Rotate()
        Item->>Item: rotationIndex = (rotationIndex + 1) % 4
        Item->>Item: UpdateRotatedShape()
        
        Note over Item: 회전된 셀 좌표 계산
        loop 각 셀
            Item->>Item: RotateCell(cell, rotationIndex)
        end
        Item->>Item: boundingSize 재계산
        
        Item->>UI: UpdateRotation()
        UI->>UI: rotation = Quaternion.Euler(0, 0, rotationIndex * 90)
        IC->>IC: rotationCommands.Push(command)
    end
    
    Note over IC: 배치 실패 시 Undo
    alt 배치 실패
        IC->>IC: UndoAllRotationCommands()
        loop rotationCommands.Count > 0
            IC->>RC: Pop().Undo()
            RC->>Item: Rotate() x 3
        end
    end
```

---

## 6. 아이템 자동 배치 시퀀스

아이템 획득 시 인벤토리의 빈 공간을 찾아 자동으로 배치하는 흐름입니다.

```mermaid
sequenceDiagram
    participant FI as FarmableItem
    participant IM as InventoryManager
    participant INV as Inventory
    participant Item as Item

    FI->>Item: new Item(itemData, stackCount)
    FI->>IM: Instance.inventory
    FI->>INV: TryAddItem(item)
    
    alt 스택 가능한 아이템
        INV->>INV: itemList 순회
        loop 기존 아이템 확인
            INV->>Item: CanStackWith(item)
            Note over Item: data == other.data &&<br/>isStackable && !IsFull
            alt 스택 가능
                INV->>Item: AddToStack(amount)
                Item->>Item: space = maxStackSize - currentStack
                Item->>Item: currentStack += min(space, amount)
                Item-->>INV: added amount
                INV->>EventBus: OnChangeInventory?.Invoke()
                alt 모든 수량 스택됨
                    INV-->>FI: true
                end
            end
        end
    end
    
    INV->>INV: TryAutoPlaceItem(item)
    
    loop Y = 0 to gridHeight
        loop X = 0 to gridWidth
            INV->>INV: CanPlaceItem(position, item)
            
            Note over INV: 각 셀 검사
            loop item.Shape 각 셀
                INV->>INV: checkX = position.x + cell.x
                INV->>INV: checkY = position.y + cell.y
                INV->>INV: 범위 검사
                INV->>INV: grid[checkX, checkY] == null 검사
            end
            
            alt 배치 가능
                INV->>INV: PlaceItem(position, item)
                INV->>INV: grid[x,y] = item
                INV->>INV: itemList.Add(item)
                INV->>EventBus: OnChangeInventory?.Invoke()
                INV-->>FI: true
            end
        end
    end
    
    INV-->>FI: false (공간 부족)
```

---

## 7. 인벤토리 UI 업데이트 시퀀스

인벤토리 변경 시 오브젝트 풀을 활용하여 UI를 효율적으로 업데이트하는 흐름입니다.

```mermaid
sequenceDiagram
    participant EB as EventBus
    participant IG as ItemGrid
    participant INV as Inventory
    participant Pool as ObjectPool
    participant UI as InventorySlotUI
    participant Item as Item

    EB->>IG: OnChangeInventory 이벤트
    IG->>IG: UpdateGrid()
    
    Note over IG: 기존 UI 정리 (오브젝트 풀 반환)
    loop activeItemObjectList
        IG->>Pool: itemObj.SetActive(false)
        IG->>Pool: itemObjectPool.Enqueue(itemObj)
    end
    IG->>IG: activeItemObjectList.Clear()
    
    Note over IG: 새 UI 생성
    IG->>INV: GetItemList()
    INV-->>IG: List<Item>
    
    loop 각 Item
        IG->>Pool: GetObjectFromPool()
        
        alt 풀에 오브젝트 있음
            Pool-->>IG: pooled object
        else 풀이 비어있음
            IG->>IG: Instantiate(ItemUIPrefab)
        end
        
        IG->>IG: itemObj.SetActive(true)
        IG->>IG: activeItemObjectList.Add(itemObj)
        
        IG->>UI: GetComponent<InventorySlotUI>()
        IG->>UI: SetItem(item, canvas)
        
        Note over UI: UI 설정
        UI->>UI: iconImage.sprite = item.data.icon
        UI->>UI: stackCountText = currentStack
        UI->>Item: SetSlotUI(this)
        UI->>UI: UpdateRotation()
        
        IG->>IG: CalculatePositionOnGrid(item)
        Note over IG: position.x = posX * tileSize + tileSize * boundingSize.x / 2<br/>position.y = -(posY * tileSize + tileSize * boundingSize.y / 2)
        IG->>UI: localPosition = position
    end
```
