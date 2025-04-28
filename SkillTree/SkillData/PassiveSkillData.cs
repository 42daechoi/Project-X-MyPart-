using UnityEngine;

[CreateAssetMenu(fileName = "PassiveSkillData", menuName = "SkillData/PassiveSkillData")]
public class PassiveSkillData : SkillData
{
    [Header("패시브 스킬 필수 설정")]
    public StatBinder effect;
}
