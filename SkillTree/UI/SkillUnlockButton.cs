using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillUnlockButton : MonoBehaviour
{
    public Skill skillToUnlock;

    private void Start()
    {
        skillToUnlock = GetComponent<Skill>();
        UpdateSkillState();
    }

    public void OnClick()
    {
        if (skillToUnlock != null)
        {
            if (!IsSkillUnlocked())
            {
                UnlockSkill();
            }
            else
            {
                LockSkill();
            }
        }
    }

    private void LockSkill()
    {
        if (skillToUnlock != null)
        {
            if (SkillManager.Instance.LockSkill(skillToUnlock))
            {
                UpdateSkillState();
            }
        }
    }

    private void UnlockSkill()
    {
        if (CheckPrerequisitesAnd(skillToUnlock) && CheckPrerequisitesOr(skillToUnlock))
        {
            if (SkillManager.Instance.UnlockSkill(skillToUnlock))
            {
                UpdateSkillState();
            }
        }
    }

    private bool IsSkillUnlocked()
    {
        var activeSkills = SkillManager.Instance.unlockedActiveSkills;
        var passiveSkills = SkillManager.Instance.unlockedPassiveSkills;

        return activeSkills.Contains(skillToUnlock) || passiveSkills.Contains(skillToUnlock);
    }

    private void UpdateSkillState()
    {
        bool unlocked = IsSkillUnlocked();
        // 스킬이 해금됐는지 체크해서 UI 업데이트 예정
    }

    private bool CheckPrerequisitesAnd(Skill skill)
    {
        List<int> reqAnd = skill.data.prerequisitesAnd;
        var active = SkillManager.Instance.unlockedActiveSkills;
        var passive = SkillManager.Instance.unlockedPassiveSkills;

        foreach (int req in reqAnd)
        {
            bool foundInActive = active.Any(s => s.data.id == req);
            bool foundInPassive = passive.Any(s => s.data.id == req);

            if (!foundInActive && !foundInPassive)
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckPrerequisitesOr(Skill skill)
    {
        List<int> reqOr = skill.data.prerequisitesOr;
        if (reqOr == null || reqOr.Count == 0)
            return true;

        var active = SkillManager.Instance.unlockedActiveSkills;
        var passive = SkillManager.Instance.unlockedPassiveSkills;

        foreach (int req in reqOr)
        {
            bool foundInActive = active.Any(s => s.data.id == req);
            bool foundInPassive = passive.Any(s => s.data.id == req);

            if (foundInActive || foundInPassive)
            {
                return true;
            }
        }

        return false;
    }
}
