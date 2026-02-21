# ⚡ 스킬 시스템 다이어그램

## 목차
1. [스킬 클래스 다이어그램](#1-스킬-클래스-다이어그램)
2. [스킬 데이터 클래스 다이어그램](#2-스킬-데이터-클래스-다이어그램)
3. [스킬 유틸리티 클래스 다이어그램](#3-스킬-유틸리티-클래스-다이어그램)
4. [스킬 인벤토리 클래스 다이어그램](#4-스킬-인벤토리-클래스-다이어그램)
5. [즉발형 스킬 사용 시퀀스](#5-즉발형-스킬-사용-시퀀스)
6. [차징형 스킬 사용 시퀀스](#6-차징형-스킬-사용-시퀀스)
7. [대쉬 스킬 시퀀스](#7-대쉬-스킬-시퀀스)
8. [스킬 해금 시퀀스](#8-스킬-해금-시퀀스)
9. [스킬 잠금 시퀀스](#9-스킬-잠금-시퀀스)
10. [스킬 데이터 로딩 시퀀스](#10-스킬-데이터-로딩-시퀀스)
11. [투사체 동작 시퀀스](#11-투사체-동작-시퀀스)

---

## 1. 스킬 클래스 다이어그램

스킬 시스템의 핵심 클래스 구조입니다. 추상 클래스 `Skill`을 기반으로 `ActiveSkill`과 `PassiveSkill`로 분기되며, 액티브 스킬은 다시 `InstantActiveSkill`(즉발형)과 `ChargingActiveSkill`(차징형)으로 나뉩니다.

```mermaid
classDiagram
    direction TB
    
    class Skill {
        <<abstract>>
        +SkillData data
        +Activate()*
    }
    
    class ActiveSkill {
        <<abstract>>
        #Transform playerTransform
        +ActiveSkillData activeData
        #int upgradeLevel
        #ActiveSkill(ActiveSkillData)
        +UpgradeSkill()
        +DowngradeSkill()
        +GetDamage() float
        +GetUpgradeLevel() int
        #GetPlayerForward() Vector2
    }
    
    class InstantActiveSkill {
        <<abstract>>
        #InstantActiveSkill(ActiveSkillData)
        +Activate()
        #OnCast()*
    }
    
    class ChargingActiveSkill {
        <<abstract>>
        #ChargingActiveSkill(ActiveSkillData)
        +Activate()
        +ActivateWithCharge(float)
        +OnChargeStart()*
        +OnCharging(float)*
        #OnCastCharged(float)* bool
        +OnChargeCancelled()*
    }
    
    class PassiveSkill {
        +PassiveSkillData passiveData
        +PassiveSkill(PassiveSkillData)
        +Activate()
        +Deactivate()
    }
    
    class SkillManager {
        <<Singleton>>
        +List~Skill~ allSkills
        +List~Skill~ unlockedActiveSkills
        +List~Skill~ unlockedPassiveSkills
        -PassiveSkillLoader passiveLoader
        -ActiveSkillLoader activeLoader
        -int currentSkillIndex
        -float rightClickStartTime
        -bool isCharge
        -Update()
        -ChargeStartOrActivate()
        -Charging()
        -ActivateChargedSkill()
        +UnlockSkill(SkillData)
        +LockSkill(SkillData)
    }
    
    class FireBallSkill {
        -GameObject chargedProjectile
        -ProjectileMover projectileMover
        +FireBallSkill(ActiveSkillData)
        +OnChargeStart()
        +OnCharging(float)
        +OnChargeCancelled()
        #OnCastCharged(float) bool
    }
    
    class FireBreathSkill {
        +FireBreathSkill(ActiveSkillData)
        #OnCast()
    }
    
    class ElectricRushSkill {
        +float dashDistanceMultiplier
        +float dashDuration
        +ElectricRushSkill(ActiveSkillData)
        #OnCast()
    }
    
    class EarthDeflectSkill {
        -float duration
        +float offset
        +EarthDeflectSkill(ActiveSkillData)
        #OnCast()
    }
    
    Skill <|-- ActiveSkill
    Skill <|-- PassiveSkill
    ActiveSkill <|-- InstantActiveSkill
    ActiveSkill <|-- ChargingActiveSkill
    ChargingActiveSkill <|-- FireBallSkill
    InstantActiveSkill <|-- FireBreathSkill
    InstantActiveSkill <|-- ElectricRushSkill
    InstantActiveSkill <|-- EarthDeflectSkill
    SkillManager --> Skill
    SkillManager --> ActiveSkillLoader
    SkillManager --> PassiveSkillLoader
```

---

## 2. 스킬 데이터 클래스 다이어그램

ScriptableObject 기반의 스킬 데이터 구조입니다. 기본 `SkillData`에서 `ActiveSkillData`와 `PassiveSkillData`로 상속됩니다.

```mermaid
classDiagram
    direction TB
    
    class SkillData {
        <<ScriptableObject>>
        +int id
        +string skillName
        +string description
        +ElementType type
    }
    
    class ActiveSkillData {
        <<ScriptableObject>>
        +StatBinder cost
        +float[] damage
        +int upgradeLevel
        +GameObject prefab
        +float projectileSpeed
        +Vector3 projectileSize
        +float maxDistance
        +float maxChargeDuration
        +GameObject targetPositionPrefab
    }
    
    class PassiveSkillData {
        <<ScriptableObject>>
        +StatBinder effect
    }
    
    class ActiveSkillLoader {
        -Dictionary~int,Func~ registry
        +LoadAllActiveSkills(Action~List~Skill~~)
    }
    
    class PassiveSkillLoader {
        +LoadAllPassiveSkills(Action~List~Skill~~)
    }
    
    class ElementType {
        <<enumeration>>
        Fire
        Electric
        Earth
    }
    
    SkillData <|-- ActiveSkillData
    SkillData <|-- PassiveSkillData
    SkillData --> ElementType
    ActiveSkillLoader --> ActiveSkillData
    PassiveSkillLoader --> PassiveSkillData
```

---

## 3. 스킬 유틸리티 클래스 다이어그램

스킬 실행에 필요한 유틸리티 클래스들입니다. 투사체 이동, 히트박스 처리, 데미지 계산 등을 담당합니다.

```mermaid
classDiagram
    direction TB
    
    class ProjectileMover {
        <<MonoBehaviour>>
        +float speed
        +float maxDistance
        +float damage
        +bool canPenetrate
        -HashSet~GameObject~ hitTargets
        -Vector2 direction
        -Vector3 startPosition
        -bool isReady
        +Init(Vector2, ActiveSkillData, float, float)
        -SetFlexData(ActiveSkillData, float)
        -Update()
        -OnTriggerEnter2D(Collider2D)
    }
    
    class SkillHitbox {
        <<MonoBehaviour>>
        +float damage
        +Vector2 overlapPosition
        +Vector2 size
        +Transform trackingTarget
        +Vector2 offset
        -List~Collider2D~ alreadyHitColliders
        -FixedUpdate()
        -ApplyDamage()
    }
    
    class TargetingProjectile {
        <<MonoBehaviour>>
        +float speed
        +float range
        +int maxHits
        -Vector2 direction
        -float damage
        -int hitCount
        -bool isHit
        +Init(Vector2, ActiveSkillData, float, int, float)
        -Update()
        -OnTriggerStay2D(Collider2D)
        -MoveToTarget()
        -FindTargetWithoutHitTarget(Collider2D[], GameObject) GameObject
    }
    
    class DamageCalculator {
        <<static>>
        +GetFinalAttackDamage(Stats, float)$ float
    }
    
    ProjectileMover --> ActiveSkillData
    ProjectileMover --> DamageCalculator
    TargetingProjectile --> ActiveSkillData
    TargetingProjectile --> DamageCalculator
    SkillHitbox --> DamageCalculator
```

---

## 4. 스킬 인벤토리 클래스 다이어그램

스킬 트리 인벤토리 관련 클래스입니다. 그리드 기반으로 스킬을 배치하고 해금하는 시스템입니다.

```mermaid
classDiagram
    direction TB
    
    class SkillInventoryLayout {
        <<ScriptableObject>>
        +int gridWidth
        +int gridHeight
        +List~CellData~ cells
    }
    
    class CellData {
        +Vector2Int position
        +ECellType cellType
        +SkillData skill
        +Vector2Int prerequisites
        +List~Vector2Int~ requiredEmptyPositions
    }
    
    class SkillInventoryLoader {
        +SkillInventoryLayout layout
        +SkillTreeInventory inventory
        +SkillInventoryLoader(SkillInventoryLayout, SkillTreeInventory)
        -PlaceCell(CellData)
        -IsValidGridPosition(Vector2Int) bool
    }
    
    class ECellType {
        <<enumeration>>
        None
        Blocked
        PassiveUnlock
        ActiveUnlock
        ConditionalBlocked
        Start
    }
    
    class SkillTreeInventory {
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
    
    SkillInventoryLayout --> CellData
    CellData --> ECellType
    CellData --> SkillData
    SkillInventoryLoader --> SkillInventoryLayout
    SkillInventoryLoader --> SkillTreeInventory
    SkillTreeInventory --> SkillInventoryLayout
```

---

## 5. 즉발형 스킬 사용 시퀀스

버튼 클릭 시 즉시 발동되는 즉발형 스킬의 실행 흐름입니다.

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

---

## 6. 차징형 스킬 사용 시퀀스

버튼을 누르고 있는 동안 차징되고, 놓으면 발동되는 차징형 스킬의 실행 흐름입니다.

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

---

## 7. 대쉬 스킬 시퀀스

ElectricRushSkill과 같은 대쉬 스킬의 실행 흐름입니다. DOTween을 사용하여 플레이어를 이동시킵니다.

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

---

## 8. 스킬 해금 시퀀스

스킬 트리 인벤토리에서 블록을 배치하여 스킬을 해금하는 흐름입니다.

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

---

## 9. 스킬 잠금 시퀀스

스킬 트리에서 블록을 제거할 때 스킬을 잠금 처리하는 흐름입니다.

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

---

## 10. 스킬 데이터 로딩 시퀀스

게임 시작 시 Addressables를 통해 스킬 데이터를 비동기 로딩하는 흐름입니다.

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

---

## 11. 투사체 동작 시퀀스

투사체(Projectile)가 생성되고 이동하며 충돌 처리하는 흐름입니다.

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
