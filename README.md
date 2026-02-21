# Project-X: Unity 게임 시스템 (인벤토리 & 스킬 트리)

## 📋 프로젝트 개요

이 프로젝트는 Unity 게임 엔진을 사용한 **그리드 기반 인벤토리 시스템**과 **스킬 트리 시스템**의 구현입니다. 주요 특징으로는:

- **테트리스 스타일의 그리드 인벤토리**: 다양한 형태의 아이템을 그리드에 배치하고 회전
- **스킬 트리 인벤토리**: 그리드 기반의 독특한 스킬 해금 시스템
- **다양한 스킬 유형**: 즉발형, 차징형, 패시브 스킬
- **속성 시스템**: 불, 전기, 땅 3가지 원소 속성

## 🎯 주요 기능

### 1. 그리드 인벤토리 시스템
- 자유 형태 아이템 배치 (테트리스 스타일)
- 아이템 드래그 앤 드롭
- 아이템 회전 (Command 패턴 적용)
- 스택 가능한 아이템 지원
- 오브젝트 풀링을 통한 UI 최적화

### 2. 스킬 트리 시스템
- 그리드 기반의 스킬 배치 및 해금
- 연결성 검증 (시작점과 연결되어야 해금 가능)
- 전제 조건 기반 스킬 해금
- 액티브/패시브 스킬 분류

### 3. 스킬 시스템
- **즉발형 스킬 (InstantActiveSkill)**: 버튼 클릭 시 즉시 발동
- **차징형 스킬 (ChargingActiveSkill)**: 차징 시간에 따라 효과 증가
- **패시브 스킬 (PassiveSkill)**: 스탯 버프 제공

## 🛠 기술 스택

- **Unity Engine** (2D)
- **C#**
- **DOTween** - 애니메이션 및 트윈
- **Addressables** - 에셋 로딩
- **ScriptableObject** - 데이터 관리

## 📁 프로젝트 구조

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

## 📊 다이어그램

자세한 다이어그램은 아래 문서를 참조하세요:

- [📐 클래스 다이어그램](./docs/CLASS_DIAGRAM.md)
- [🔄 시퀀스 다이어그램](./docs/SEQUENCE_DIAGRAM.md)
- [📁 구조 다이어그램](./docs/STRUCTURE_DIAGRAM.md)

## 👨‍💻 담당 역할

이 프로젝트에서 담당한 부분:
- ✅ 그리드 기반 인벤토리 시스템 설계 및 구현
- ✅ 스킬 트리 시스템 설계 및 구현
- ✅ 다양한 액티브 스킬 구현 (불, 전기, 땅 속성)
- ✅ 스킬 데이터 관리 시스템 (ScriptableObject)
- ✅ 에디터 확장 도구 개발

## 📞 Contact

- GitHub: [@42daechoi](https://github.com/42daechoi)
