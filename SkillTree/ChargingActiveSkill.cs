using UnityEngine;

public abstract class ChargingActiveSkill : ActiveSkill
{
    protected ChargingActiveSkill(ActiveSkillData activeSkillData) : base(activeSkillData)
    {
    }

    public override void Activate() { }

    public void ActivateWithCharge(float chargeDuration)
    {
        OnCastCharged(chargeDuration);
    }


    public abstract void OnChargeStart();

    public abstract void OnCharging(float chargeDuration);

    protected abstract bool OnCastCharged(float chargeDuration);

    public abstract void OnChargeCancelled();

}