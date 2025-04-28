using UnityEngine;

public class PassiveSkill : Skill
{
    [HideInInspector] public PassiveSkillData passiveData;
    [HideInInspector] private PlayerController playerController;

    private void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        if (data is PassiveSkillData casted)
        {
            passiveData = casted;
        }
        else
        {
            Debug.LogError($"PassiveSkill : {data.name}�� �Ҵ�� SkillData�� PassiveSkillData�� �ƴմϴ�.");
        }
    }

    public override void Activate()
    {
        playerController.stats = playerController.stats + passiveData.effect;
    }

    public void Deactivate()
    {
        playerController.stats = playerController.stats - passiveData.effect;
    }
}
