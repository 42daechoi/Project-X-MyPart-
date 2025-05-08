using UnityEngine;

public abstract class InstantActiveSkill : ActiveSkill
{
    protected InstantActiveSkill(ActiveSkillData data) : base(data) { }

    public override void Activate()
    {
        OnCast();
    }

    protected abstract void OnCast();
}
