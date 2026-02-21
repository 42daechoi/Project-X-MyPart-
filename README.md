```
Project-X-MyPart-/
├── Editor/                     # Unity 에디터 확장
│   ├── ActiveSkillDataImporter.cs
│   ├── SkillInventoryEditor.cs
│   └── SkillInventoryLayoutEditor.cs
├── Enums/                      # 열거형 정의
│   ├── ElementType.cs         # 원소 속성 (Fire, Electric, Earth)
│   └── Enums.cs               # 게임 관련 열거형
├── GridInventory/              # 그리드 인벤토리 시스템
│   ├── Inventory.cs           # 기본 인벤토리 클래스
│   ├── InventoryController.cs # 드래그&드롭 컨트롤러
│   ├── InventoryManager.cs    # 인벤토리 관리자 (싱글톤)
│   ├── ItemGrid.cs            # UI 그리드 컴포넌트
│   ├── ICommand.cs            # 커맨드 패턴 인터페이스
│   ├── RotateCommand.cs       # 회전 커맨드
│   └── SkillInventory/        # 스킬 전용 인벤토리
│       ├── SkillTreeInventory.cs
│       ├── SkillInventoryLayout.cs
│       └── SkillInventoryLoader.cs
├── Item/                       # 아이템 시스템
│   ├── Item.cs                # 아이템 인스턴스
│   ├── ItemData.cs            # 아이템 기본 데이터 (SO)
│   ├── GearData.cs            # 장비 데이터
│   ├── GemData.cs             # 보석 데이터
│   └── ComsumableItemData.cs  # 소모품 데이터
├── SkillTree/                  # 스킬 시스템
│   ├── Skill.cs               # 스킬 기본 클래스
│   ├── ActiveSkill.cs         # 액티브 스킬 추상 클래스
│   ├── PassiveSkill.cs        # 패시브 스킬
│   ├── InstantActiveSkill.cs  # 즉발형 스킬
│   ├── ChargingActiveSkill.cs # 차징형 스킬
│   ├── SkillManager.cs        # 스킬 관리자 (싱글톤)
│   ├── SkillData/             # 스킬 데이터 (SO)
│   ├── ActiveSkills/          # 개별 스킬 구현
│   ├── ActiveUtils/           # 스킬 유틸리티
│   └── PassiveUtils/          # 패시브 유틸리티
└── Structs/                    # 구조체 정의
    └── Stats.cs               # 스탯 구조체
```
## 🔑 핵심 설계 패턴
### 1. 싱글톤 패턴 (Singleton)
- `InventoryManager`, `SkillManager`에서 전역 접근 제공
### 2. 커맨드 패턴 (Command)
- `ICommand` 인터페이스와 `RotateCommand`를 통한 되돌리기(Undo) 기능
### 3. 오브젝트 풀 패턴 (Object Pool)
- `ItemGrid`, `SlotUIHighlighter`에서 UI 요소 재사용
### 4. 팩토리 패턴 (Factory)
- `ActiveSkillLoader`에서 스킬 ID에 따른 스킬 인스턴스 생성
### 5. 전략 패턴 (Strategy)
- 스킬 타입별 다형성 (`InstantActiveSkill`, `ChargingActiveSkill`)
### 6. ScriptableObject 패턴
- 데이터와 로직 분리를 통한 확장성
