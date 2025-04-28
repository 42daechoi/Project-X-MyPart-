public static class DamageCalculator
{
    public static float GetFinalAttackDamage(Stats stats, float skillMultiplier)
    {
        float finalDamage = stats.attackPower * (1 + skillMultiplier) * (1 + stats.additionalAttackPowerRate);
        return finalDamage;
    }

    public static float GetFinalTakeDamage(Stats stats, float damage, bool isPassiveOn = false)
    {
        float finalDamage = damage - stats.defense;
        if (isPassiveOn)
        {
            finalDamage = finalDamage * (1 - stats.additionalDefensePowerRate);
        }

        return finalDamage;
    }

}
