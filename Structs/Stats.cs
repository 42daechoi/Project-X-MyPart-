public enum StatType
{
    MeleePower,
    MaxHealth,
    CurHealth,
    MoveSpeed,
    CurFireGauge,
    MaxFireGauge,
    FireRecovery,
    AdditionalFirePower,
    CurElectricGauge,
    MaxElectricGauge,
    ElectricRecovery,
    AdditionalElectricPower,
    CurEarthGauge,
    MaxEarthGauge,
    EarthRecovery,
    AdditionalEarthPower,
    CurHpGauge,
    MaxHpGauge,
    hpRecovery,
    AdditionalMeleePowerRate,
    AdditionalDefensePowerRate,
    MiningSpeed,
    PickAxeLevel
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
    public float meleePower;
    public float maxHealth;
    public float curHealth;
    public float moveSpeed;

    public float curFireGauge;
    public float maxFireGauge;
    public float fireRecovery;
    public float additionalFirePower;

    public float curElectricGauge;
    public float maxElectricGauge;
    public float electricRecovery;
    public float additionalElectricPower;

    public float curEarthGauge;
    public float maxEarthGauge;
    public float earthRecovery;
    public float additionalEarthPower;

    public float curHPGauge;
    public float maxHPGauge;
    public float hpRecovery;

    public float additionalMeleePowerRate;
    public float additionalDefensePowerRate;

    public float miningSpeed;
    public int pickAxeLevel;

    public static Stats Zero => new Stats();

    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats
        {
            meleePower = a.meleePower + b.meleePower,
            maxHealth = a.maxHealth + b.maxHealth,
            curHealth = a.curHealth + b.curHealth,
            moveSpeed = a.moveSpeed + b.moveSpeed,
            curFireGauge = a.curFireGauge + b.curFireGauge,
            maxFireGauge = a.maxFireGauge + b.maxFireGauge,
            fireRecovery = a.fireRecovery + b.fireRecovery,
            additionalFirePower = a.additionalFirePower + b.additionalFirePower,
            curElectricGauge = a.curElectricGauge + b.curElectricGauge,
            maxElectricGauge = a.maxElectricGauge + b.maxElectricGauge,
            electricRecovery = a.electricRecovery + b.electricRecovery,
            additionalElectricPower = a.additionalElectricPower + b.additionalElectricPower,
            curEarthGauge = a.curEarthGauge + b.curEarthGauge,
            maxEarthGauge = a.maxEarthGauge + b.maxEarthGauge,
            earthRecovery = a.earthRecovery + b.earthRecovery,
            additionalEarthPower = a.additionalEarthPower + b.additionalEarthPower,
            curHPGauge = a.curHPGauge + b.curHPGauge,
            maxHPGauge = a.maxHPGauge + b.maxHPGauge,
            hpRecovery = a.hpRecovery + b.hpRecovery,
            additionalMeleePowerRate = a.additionalMeleePowerRate + b.additionalMeleePowerRate,
            additionalDefensePowerRate = a.additionalDefensePowerRate + b.additionalDefensePowerRate,
            miningSpeed = a.miningSpeed + b.miningSpeed,
            pickAxeLevel = a.pickAxeLevel + b.pickAxeLevel,
        };
    }

    public static Stats operator -(Stats a, Stats b)
    {
        return new Stats
        {
            meleePower = a.meleePower - b.meleePower,
            maxHealth = a.maxHealth - b.maxHealth,
            curHealth = a.curHealth - b.curHealth,
            moveSpeed = a.moveSpeed - b.moveSpeed,
            curFireGauge = a.curFireGauge - b.curFireGauge,
            maxFireGauge = a.maxFireGauge - b.maxFireGauge,
            fireRecovery = a.fireRecovery - b.fireRecovery,
            additionalFirePower = a.additionalFirePower - b.additionalFirePower,
            curElectricGauge = a.curElectricGauge - b.curElectricGauge,
            maxElectricGauge = a.maxElectricGauge - b.maxElectricGauge,
            electricRecovery = a.electricRecovery - b.electricRecovery,
            additionalElectricPower = a.additionalElectricPower - b.additionalElectricPower,
            curEarthGauge = a.curEarthGauge - b.curEarthGauge,
            maxEarthGauge = a.maxEarthGauge - b.maxEarthGauge,
            earthRecovery = a.earthRecovery - b.earthRecovery,
            additionalEarthPower = a.additionalEarthPower - b.additionalEarthPower,
            curHPGauge = a.curHPGauge - b.curHPGauge,
            maxHPGauge = a.maxHPGauge - b.maxHPGauge,
            hpRecovery = a.hpRecovery - b.hpRecovery,
            additionalMeleePowerRate = a.additionalMeleePowerRate - b.additionalMeleePowerRate,
            additionalDefensePowerRate = a.additionalDefensePowerRate - b.additionalDefensePowerRate,
            miningSpeed = a.miningSpeed - b.miningSpeed,
            pickAxeLevel = a.pickAxeLevel - b.pickAxeLevel,
        };
    }

    public static Stats operator +(Stats stats, StatBinder binder)
    {
        switch (binder.type)
        {
            case StatType.MeleePower: stats.meleePower += binder.value; break;
            case StatType.MaxHealth: stats.maxHealth += binder.value; break;
            case StatType.CurHealth: stats.curHealth += binder.value; break;
            case StatType.MoveSpeed: stats.moveSpeed += binder.value; break;
            case StatType.CurFireGauge: stats.curFireGauge += binder.value; break;
            case StatType.MaxFireGauge: stats.maxFireGauge += binder.value; break;
            case StatType.FireRecovery: stats.fireRecovery += binder.value; break;
            case StatType.AdditionalFirePower: stats.additionalFirePower += binder.value; break;
            case StatType.CurElectricGauge: stats.curElectricGauge += binder.value; break;
            case StatType.MaxElectricGauge: stats.maxElectricGauge += binder.value; break;
            case StatType.ElectricRecovery: stats.electricRecovery += binder.value; break;
            case StatType.AdditionalElectricPower: stats.additionalElectricPower += binder.value; break;
            case StatType.CurEarthGauge: stats.curEarthGauge += binder.value; break;
            case StatType.MaxEarthGauge: stats.maxEarthGauge += binder.value; break;
            case StatType.EarthRecovery: stats.earthRecovery += binder.value; break;
            case StatType.AdditionalEarthPower: stats.additionalEarthPower += binder.value; break;
            case StatType.CurHpGauge: stats.curHPGauge += binder.value; break;
            case StatType.MaxHpGauge: stats.maxHPGauge += binder.value; break;
            case StatType.hpRecovery: stats.hpRecovery += binder.value; break;
            case StatType.AdditionalMeleePowerRate: stats.additionalMeleePowerRate += binder.value; break;
            case StatType.AdditionalDefensePowerRate: stats.additionalDefensePowerRate += binder.value; break;
            case StatType.MiningSpeed: stats.miningSpeed += binder.value; break;
            case StatType.PickAxeLevel: stats.pickAxeLevel += (int)binder.value; break;
        }
        return stats;
    }

    public static Stats operator -(Stats stats, StatBinder binder)
    {
        switch (binder.type)
        {
            case StatType.MeleePower: stats.meleePower -= binder.value; break;
            case StatType.MaxHealth: stats.maxHealth -= binder.value; break;
            case StatType.CurHealth: stats.curHealth -= binder.value; break;
            case StatType.MoveSpeed: stats.moveSpeed -= binder.value; break;
            case StatType.CurFireGauge: stats.curFireGauge -= binder.value; break;
            case StatType.MaxFireGauge: stats.maxFireGauge -= binder.value; break;
            case StatType.FireRecovery: stats.fireRecovery -= binder.value; break;
            case StatType.AdditionalFirePower: stats.additionalFirePower -= binder.value; break;
            case StatType.CurElectricGauge: stats.curElectricGauge -= binder.value; break;
            case StatType.MaxElectricGauge: stats.maxElectricGauge -= binder.value; break;
            case StatType.ElectricRecovery: stats.electricRecovery -= binder.value; break;
            case StatType.AdditionalElectricPower: stats.additionalElectricPower -= binder.value; break;
            case StatType.CurEarthGauge: stats.curEarthGauge -= binder.value; break;
            case StatType.MaxEarthGauge: stats.maxEarthGauge -= binder.value; break;
            case StatType.EarthRecovery: stats.earthRecovery -= binder.value; break;
            case StatType.AdditionalEarthPower: stats.additionalEarthPower -= binder.value; break;
            case StatType.CurHpGauge: stats.curHPGauge -= binder.value; break;
            case StatType.MaxHpGauge: stats.maxHPGauge -= binder.value; break;
            case StatType.hpRecovery: stats.hpRecovery -= binder.value; break;
            case StatType.AdditionalMeleePowerRate: stats.additionalMeleePowerRate -= binder.value; break;
            case StatType.AdditionalDefensePowerRate: stats.additionalDefensePowerRate -= binder.value; break;
            case StatType.MiningSpeed: stats.miningSpeed -= binder.value; break;
            case StatType.PickAxeLevel: stats.pickAxeLevel -= (int)binder.value; break;
        }
        return stats;
    }
}
