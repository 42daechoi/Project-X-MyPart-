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
            Debug.LogError("ActiveSkill : Player�� ã�� �� �����ϴ�.");
        }
        else
        {
            playerTransform = player.transform;
            if (playerTransform == null)
            {
                Debug.LogError("ActiveSkill : playerTransform�� ã�� �� �����ϴ�.");
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
            Debug.LogError("ActiveSkill : ��ų �����Ϳ� ������ ����Ʈ�� ������ �ֽ��ϴ�.");
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
