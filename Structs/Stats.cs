public enum StatType
{
    AttackPower,
    Defense,
    MovementSpeed,
    Health,
    Mana,
    Stamina,
    AttackSpeed,
    AdditionalAttackPowerRate,
    AdditionalDefensePowerRate,
    AdditionalCooldownRedution,
    AdditionalHealthPotionRate,
    MiningBonus,
    MiningSpeed
}

[System.Serializable]
public struct StatBinder
{
    public StatType type;
    public float value;
}

[System.Serializable]
public struct Stats
{
    public float attackPower;
    public float defense;
    public float moveSpeed;
    public float maxHealth;
    public float curHealth;
    public float maxMana;
    public float curMana;
    public float stamina;
    public float attackSpeed;
    public float miningBonus;
    public float miningSpeed;

    public float additionalAttackPowerRate;
    public float additionalDefensePowerRate;
    public float additionalCooldownRedution;
    public float additionalHealthPotionRate;

    public static Stats Zero => new Stats
    {
        attackPower = 0,
        defense = 0,
        moveSpeed = 0,
        maxHealth = 0,
        curHealth = 0,
        maxMana = 0,
        curMana = 0,
        stamina = 0,
        attackSpeed = 0,
        additionalAttackPowerRate = 0,
        additionalDefensePowerRate = 0,
        additionalCooldownRedution = 0,
        additionalHealthPotionRate = 0,
        miningBonus = 0,
        miningSpeed = 0
    };

    /// <summary>
    /// 장비의 추가 스텟 합산
    /// </summary>
    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats
        {
            attackPower = a.attackPower + b.attackPower,
            defense = a.defense + b.defense,
            moveSpeed = a.moveSpeed + a.moveSpeed * b.moveSpeed,
            maxHealth = a.maxHealth + b.maxHealth,
            maxMana = a.maxMana + b.maxMana,
            stamina = a.stamina + b.stamina,
            attackSpeed = a.attackSpeed + b.attackSpeed,
            miningBonus = a.miningBonus + b.miningBonus,
            miningSpeed = a.miningSpeed + b.miningSpeed
        };
    }

    /// <summary>
    /// 장비의 추가 스텟 차감
    /// </summary>
    public static Stats operator -(Stats a, Stats b)
    {
        return new Stats
        {
            attackPower = a.attackPower - b.attackPower,
            defense = a.defense - b.defense,
            moveSpeed = a.moveSpeed - b.moveSpeed,
            maxHealth = a.maxHealth - b.maxHealth,
            maxMana = a.maxMana - b.maxMana,
            stamina = a.stamina - b.stamina,
            attackSpeed = a.attackSpeed - b.attackSpeed,
            miningBonus = a.miningBonus - b.miningBonus,
            miningSpeed = a.miningSpeed - b.miningSpeed
        };
    }

    /// <summary>
    /// 소모품 사용 및 스킬트리 추가 스텟 합산
    /// </summary>
    public static Stats operator +(Stats stats, StatBinder binder)
    {
        switch (binder.type)
        {
            case StatType.Health:
                stats.curHealth += stats.maxHealth * (binder.value + stats.additionalHealthPotionRate);
                break;
            case StatType.Mana:
                stats.curMana += stats.maxMana * binder.value;
                break;
            case StatType.AdditionalAttackPowerRate:
                stats.additionalAttackPowerRate += binder.value;
                break;
            case StatType.AdditionalDefensePowerRate:
                stats.additionalDefensePowerRate += binder.value;
                break;
            case StatType.AdditionalCooldownRedution:
                stats.additionalCooldownRedution += binder.value;
                break;
            case StatType.AdditionalHealthPotionRate:
                stats.additionalHealthPotionRate += binder.value;
                break;
            case StatType.MiningBonus:
              stats.miningBonus += binder.value;
                break;
            case StatType.MiningSpeed:
              stats.miningSpeed += binder.value;
                break;
            default:
                break;
        }
        return stats;
    }

    /// <summary>
    /// 스킬트리 추가 스텟 차감
    /// </summary>
    public static Stats operator -(Stats stats, StatBinder binder)
    {
        switch (binder.type)
        {
            case StatType.AdditionalAttackPowerRate:
                stats.additionalAttackPowerRate -= binder.value;
                break;
            case StatType.AdditionalDefensePowerRate:
                stats.additionalDefensePowerRate -= binder.value;
                break;
            case StatType.AdditionalCooldownRedution:
                stats.additionalCooldownRedution -= binder.value;
                break;
            case StatType.AdditionalHealthPotionRate:
                stats.additionalHealthPotionRate -= binder.value;
                break;
            case StatType.MiningBonus:
                stats.miningBonus -= binder.value;
                break;
            case StatType.MiningSpeed:
                stats.miningSpeed -= binder.value;
                break;
            default:
                break;
        }
        return stats;
    }
}
