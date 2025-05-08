using System;
using UnityEngine;

public enum EEnemyType
{
    Skeleton,
    PlantMonster
}

public enum EPortalType
{
    Village,
    Stage1_1,
    Stage1_2,
    Stage1_3,
    Boss
}

public enum ECheckpoint
{
    Village,
    Stage1_1,
    Stage1_2,
    Stage1_3
}

public enum ERelicType
{
    Dash,
    WallJump,
    Gills,
    Luminance,
    Glide
}


public enum EInventoryType
{
    Item,
    SkillTree
}

public enum ECellType
{
    None,
    Blocked,
    PassiveUnlock,
    ActiveUnlock,
    ConditionalBlocked,
    Start
}