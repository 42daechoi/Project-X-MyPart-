using UnityEngine;

public abstract class ChargingActiveSkill : ActiveSkill
{
    public override void Activate() { }

    public void ActivateWithCharge(float chargeDuration)
    {
        if (OnCastCharged(chargeDuration))
        {
            lastUseTime = Time.time;
        }
    }

    protected bool CanStartCharge()
    {
        float cooldown = GetCooldown();
        if (Time.time < lastUseTime + cooldown) return false;
        return true;
    }

    // OnChargeStart 호출 시 CanStartCharge 필수 확인
    public abstract void OnChargeStart();

    public abstract void OnCharging(float chargeDuration);

    protected abstract bool OnCastCharged(float chargeDuration);

    public abstract void OnChargeCancelled();

}