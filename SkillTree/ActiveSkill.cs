using UnityEngine;

public abstract class ActiveSkill : Skill
{
    protected Transform playerTransform;
    [HideInInspector] public ActiveSkillData activeData;
    protected int upgradeLevel = 0;

    protected ActiveSkill(ActiveSkillData activeSkillData)
    {
        activeData = activeSkillData;
        data = activeSkillData;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("ActiveSkill : Player를 찾을 수 없습니다.");
        }
        else
        {
            playerTransform = player.transform;
            if (playerTransform == null)
            {
                Debug.LogError("ActiveSkill : playerTransform을 찾을 수 없습니다.");
            }
        }
    }

    public void UpgradeSkill()
    {
        if (upgradeLevel < 2) upgradeLevel++;
    }

    public void DowngradeSkill()
    {
        if (upgradeLevel > 0) upgradeLevel--;
    }

    public float GetDamage()
    {
        if (activeData.damage.Length != 3)
        {
            Debug.LogError("ActiveSkill : 스킬 데이터에 데미지 리스트에 문제가 있습니다.");
            return 0f;
        }
        return activeData.damage[upgradeLevel];
    }

    public int GetUpgradeLevel()
    {
        return upgradeLevel;
    }

    protected Vector2 GetPlayerForward()
    {
        Vector2 forward = playerTransform.localScale.x > 0 ? Vector2.right : Vector2.left;
        bool isGravityInverted = playerTransform.up.y < 0;
        if (isGravityInverted)
        {
            forward *= -1;
        }
        return forward;
    }
}
