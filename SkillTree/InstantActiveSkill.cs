using UnityEngine;

public abstract class InstantActiveSkill : ActiveSkill
{
    public override void Activate()
    {
        float cooldown = GetCooldown();
        if (Time.time < lastUseTime + cooldown) return;
        lastUseTime = Time.time;
        OnCast();
    }

    protected abstract void OnCast();
}
