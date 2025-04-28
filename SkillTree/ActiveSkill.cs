using UnityEngine;

public abstract class ActiveSkill : Skill
{
    [HideInInspector] public ActiveSkillData activeData;
    protected float upgradeLevel = 1;
    protected float lastUseTime = -Mathf.Infinity;

    private void Awake()
    {
        if (data is ActiveSkillData casted)
        {
            activeData = casted;
        }
        else
        {
            Debug.LogError($"ActiveSkill : {name}에 할당된 SkillData가 ActiveSkillData가 아닙니다.");
        }
    }

    protected Transform GetPlayerTransform()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player != null ? player.transform : null;
    }

    protected float GetCooldown()
    {
        return activeData.cooldown;
    }

    public float GetLevel() => upgradeLevel;

    public void UpgradeSkill()
    {
        if (upgradeLevel < 3) upgradeLevel++;
    }

    public void DowngradeSkill()
    {
        if (upgradeLevel > 1) upgradeLevel--;
    }
}
