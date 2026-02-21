# 🔄 시퀀스 다이어그램

## 1. 인벤토리 아이템 배치 시퀀스

### 1.1 아이템 드래그 앤 드롭

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

### 1.2 아이템 회전

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
        Item->>UI: UpdateRotation()
        UI->>UI: rotation = Quaternion.Euler(0, 0, rotationIndex * 90)
        IC->>IC: rotationCommands.Push(command)
    end
```

### 1.3 아이템 자동 배치 (획득 시)

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
            alt 스택 가능
                INV->>Item: AddToStack(amount)
                Item-->>INV: added amount
                INV->>EventBus: OnChangeInventory?.Invoke()
            end
        end
    end
    
    INV->>INV: TryAutoPlaceItem(item)
    loop Y = 0 to gridHeight
        loop X = 0 to gridWidth
            INV->>INV: CanPlaceItem(position, item)
            alt 배치 가능
                INV->>INV: PlaceItem(position, item)
                INV-->>FI: true
            end
        end
    end
    
    INV-->>FI: false (공간 부족)
```

## 2. 스킬 사용 시퀀스

### 2.1 즉발형 스킬 사용

```mermaid
sequenceDiagram
    participant User as 👤 User
    participant SM as SkillManager
    participant IS as InstantActiveSkill
    participant Skill as 구체적 스킬<br/>(FireBreathSkill 등)
    participant GO as GameObject
    participant SH as SkillHitbox

    User->>SM: 우클릭 (MouseButtonDown(1))
    SM->>SM: ChargeStartOrActivate()
    SM->>SM: unlockedActiveSkills[currentSkillIndex]
    
    alt InstantActiveSkill인 경우
        SM->>IS: Activate()
        IS->>Skill: OnCast()
        
        Note over Skill: FireBreathSkill 예시
        Skill->>Skill: GetPlayerForward()
        Skill->>GO: Instantiate(prefab, position)
        GO-->>Skill: hitbox GameObject
        Skill->>SH: GetComponent<SkillHitbox>()
        Skill->>SH: size = (width, height)
        Skill->>SH: damage = GetDamage()
        Skill->>SH: trackingTarget = playerTransform
        
        Skill->>DOTween: DelayedCall(duration, Destroy)
        
        SM->>SM: MoveToNextSkill()
        SM->>SM: currentSkillIndex = (index + 1) % count
    end
```

### 2.2 차징형 스킬 사용

```mermaid
sequenceDiagram
    participant User as 👤 User
    participant SM as SkillManager
    participant CS as ChargingActiveSkill
    participant FB as FireBallSkill
    participant PM as ProjectileMover
    participant GO as GameObject

    User->>SM: 우클릭 시작 (MouseButtonDown(1))
    SM->>SM: ChargeStartOrActivate()
    
    alt ChargingActiveSkill인 경우
        SM->>SM: rightClickStartTime = Time.time
        SM->>SM: isCharge = true
        SM->>FB: OnChargeStart()
        
        FB->>FB: UpdateProjectilePositions()
        FB->>GO: Instantiate(prefab, spawnPosition)
        GO-->>FB: chargedProjectile
        FB->>PM: GetComponent<ProjectileMover>()
        FB->>PM: enabled = false
        FB->>FB: chargedProjectile.localScale = minScale
    end
    
    loop 차징 중 (isCharge && MouseButton(1))
        User->>SM: Update()
        SM->>SM: Charging()
        SM->>FB: OnCharging(currentChargeDuration)
        FB->>FB: chargeRatio = duration / maxChargeDuration
        FB->>GO: localScale = Lerp(minScale, maxSize, ratio)
    end
    
    User->>SM: 우클릭 해제 (MouseButtonUp(1))
    SM->>SM: ActivateChargedSkill()
    SM->>SM: isCharge = false
    SM->>SM: chargeDuration = Time.time - startTime
    SM->>FB: ActivateWithCharge(chargeDuration)
    
    FB->>FB: OnCastCharged(chargeDuration)
    FB->>FB: 최종 스케일 계산
    FB->>PM: enabled = true
    FB->>PM: Init(direction, data, duration, damage)
    PM->>PM: 발사체 이동 시작
    
    SM->>SM: MoveToNextSkill()
```

### 2.3 대쉬 스킬 (ElectricRushSkill)

```mermaid
sequenceDiagram
    participant User as 👤 User
    participant SM as SkillManager
    participant ER as ElectricRushSkill
    participant Player as PlayerTransform
    participant GO as GameObject
    participant SH as SkillHitbox
    participant DOT as DOTween

    User->>SM: 우클릭
    SM->>ER: Activate()
    ER->>ER: OnCast()
    
    ER->>Player: GetComponent<Collider2D>()
    ER->>ER: dashDistance = playerWidth * multiplier
    ER->>ER: GetPlayerForward()
    ER->>ER: dashEndPos = startPos + direction * distance
    
    ER->>GO: Instantiate(prefab, startPos)
    GO-->>ER: hitbox
    ER->>SH: GetComponent<SkillHitbox>()
    ER->>SH: size = (width, height)
    ER->>SH: damage = CalculatedDamage
    ER->>SH: trackingTarget = playerTransform
    
    ER->>DOT: DOMove(dashEndPos, duration)
    DOT->>Player: 대쉬 이동 애니메이션
    
    DOT->>DOT: OnComplete()
    DOT->>GO: Destroy(hitbox, 0.1f)
```

## 3. 스킬 해금 시퀀스

### 3.1 스킬 트리에서 스킬 해금

```mermaid
sequenceDiagram
    participant User as 👤 User
    participant IC as InventoryController
    participant STI as SkillTreeInventory
    participant SM as SkillManager
    participant Skill as Skill
    participant PS as PassiveSkill
    participant PC as PlayerController

    User->>IC: 아이템 드롭 (테트리스 블록)
    IC->>STI: PlaceItem(position, item)
    
    STI->>STI: CanPlaceItem(position, item)
    
    Note over STI: 배치 검증
    STI->>STI: base.CanPlaceItem() 호출
    STI->>STI: cellType 검증
    STI->>STI: IsConnectedToStart() 검증
    
    alt 배치 가능
        STI->>STI: base.PlaceItem() 호출
        STI->>STI: grid[x,y] = item
        
        loop 각 셀 확인
            STI->>STI: cellType = cellTypes[x,y]
            
            alt ActiveUnlock 또는 PassiveUnlock
                STI->>STI: GetSkillDataAt(position)
                STI->>SM: UnlockSkill(skillData)
                
                SM->>SM: GetSkillById(id)
                SM->>SM: GetTargetList(skill)
                
                alt 이미 해금된 스킬
                    SM->>SM: UpgradeSkillLevel()
                else 새로운 스킬
                    SM->>SM: targetList.Add(skill)
                    SM->>SM: ApplyPassiveStats(skill, true)
                    
                    alt PassiveSkill인 경우
                        SM->>PS: Activate()
                        PS->>PC: stats = stats + effect
                    end
                end
            end
        end
        
        STI-->>IC: true
    else 배치 불가능
        STI-->>IC: false
    end
```

### 3.2 스킬 잠금 (블록 제거)

```mermaid
sequenceDiagram
    participant User as 👤 User
    participant IC as InventoryController
    participant STI as SkillTreeInventory
    participant SM as SkillManager
    participant AS as ActiveSkill
    participant PS as PassiveSkill
    participant PC as PlayerController

    User->>IC: 아이템 픽업
    IC->>STI: RemoveItemAt(position, isReplace)
    
    STI->>STI: GetItemAt(position)
    STI->>STI: GetTempOccupied()
    
    Note over STI: 제거 후 연결성 검증
    STI->>STI: occupied.Remove(itemCells)
    STI->>STI: IsConnectedToStart(occupied)
    
    alt 연결 유지됨
        STI->>STI: CheckCondition(item, position)
        
        alt 조건 충족
            loop 각 셀 확인
                STI->>STI: cellType = cellTypes[x,y]
                
                alt PassiveUnlock 또는 ActiveUnlock
                    STI->>SM: LockSkill(skillData)
                    
                    SM->>SM: GetSkillById(id)
                    SM->>SM: GetTargetList(skill)
                    
                    alt ActiveSkill && upgradeLevel > 0
                        SM->>AS: DowngradeSkill()
                        AS->>AS: upgradeLevel--
                    else 레벨 0 또는 PassiveSkill
                        SM->>SM: ApplyPassiveStats(skill, false)
                        
                        alt PassiveSkill인 경우
                            SM->>PS: Deactivate()
                            PS->>PC: stats = stats - effect
                        end
                        
                        SM->>SM: targetList.Remove(skill)
                    end
                end
                
                STI->>STI: grid[x,y] = null
            end
            
            STI->>EventBus: OnChangeInventory?.Invoke()
            STI-->>IC: true
        else 조건 불충족
            STI-->>IC: false
        end
    else 연결 끊김
        STI-->>IC: false
    end
```

## 4. 스킬 데이터 로딩 시퀀스

### 4.1 게임 시작 시 스킬 로딩

```mermaid
sequenceDiagram
    participant Unity as Unity Engine
    participant SM as SkillManager
    participant ASL as ActiveSkillLoader
    participant PSL as PassiveSkillLoader
    participant ADDR as Addressables
    participant SO as ScriptableObjects

    Unity->>SM: Start()
    
    par 병렬 로딩
        SM->>PSL: LoadAllPassiveSkills(callback)
        SM->>ASL: LoadAllActiveSkills(callback)
    end
    
    PSL->>ADDR: LoadAssetsAsync<PassiveSkillData>("PassiveSkills")
    ADDR->>SO: 패시브 스킬 데이터 로드
    SO-->>ADDR: List<PassiveSkillData>
    
    loop 각 PassiveSkillData
        PSL->>PSL: new PassiveSkill(data)
        PSL->>PSL: loadedSkills.Add(skill)
    end
    
    PSL-->>SM: OnSkillsLoaded(passiveSkills)
    SM->>SM: allSkills.AddRange(list)
    
    ASL->>ADDR: LoadAssetsAsync<ActiveSkillData>("ActiveSkills")
    ADDR->>SO: 액티브 스킬 데이터 로드
    SO-->>ADDR: List<ActiveSkillData>
    
    loop 각 ActiveSkillData
        ASL->>ASL: registry[id]로 스킬 생성
        Note over ASL: 팩토리 패턴으로<br/>ID에 맞는 스킬 인스턴스 생성
        ASL->>ASL: loadedSkills.Add(skill)
    end
    
    ASL-->>SM: OnSkillsLoaded(activeSkills)
    SM->>SM: allSkills.AddRange(list)
    SM->>SM: UpdateSkillViewList()
```

## 5. 인벤토리 UI 업데이트 시퀀스

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
    
    Note over IG: 기존 UI 정리
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
        UI->>Item: SetSlotUI(this)
        UI->>UI: UpdateRotation()
        
        IG->>IG: CalculatePositionOnGrid(item)
        IG->>UI: localPosition = position
    end
```

## 6. 투사체 동작 시퀀스

```mermaid
sequenceDiagram
    participant Skill as ActiveSkill
    participant GO as GameObject
    participant PM as ProjectileMover
    participant Enemy as Enemy
    participant Ground as Ground

    Skill->>GO: Instantiate(prefab)
    Skill->>PM: Init(direction, data, charge, damage)
    PM->>PM: isReady = true
    
    loop Update (매 프레임)
        PM->>PM: position += direction * speed * deltaTime
        PM->>PM: traveled = Distance(start, current)
        
        alt 최대 거리 도달
            PM->>GO: Destroy(gameObject)
        end
    end
    
    alt 적과 충돌 (OnTriggerEnter2D)
        Enemy->>PM: OnTriggerEnter2D(collider)
        
        alt 관통 불가
            PM->>Enemy: TakeDamage(damage)
            PM->>GO: Destroy(gameObject)
        else 관통 가능
            alt 첫 피격
                PM->>PM: hitTargets.Add(enemy)
                PM->>Enemy: TakeDamage(damage)
            end
        end
    end
    
    alt 지형과 충돌
        Ground->>PM: OnTriggerEnter2D(collider)
        PM->>GO: Destroy(gameObject)
    end
```
