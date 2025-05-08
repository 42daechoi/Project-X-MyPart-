using UnityEngine;

public class PassiveSkill : Skill
{
    [HideInInspector] public PassiveSkillData passiveData;

    public PassiveSkill(PassiveSkillData passiveSkillData)
    {
        data = passiveSkillData;
        passiveData = passiveSkillData;
    }

    public override void Activate()
    {
        PlayerController.Instance.stats = PlayerController.Instance.stats + passiveData.effect;
    }

    public void Deactivate()
    {
        PlayerController.Instance.stats = PlayerController.Instance.stats - passiveData.effect;
    }
}
