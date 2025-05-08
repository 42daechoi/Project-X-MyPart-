using UnityEngine;

public class ElectricPenetrateShotSkill : InstantActiveSkill
{
    public ElectricPenetrateShotSkill(ActiveSkillData data) : base(data)
    {
    }

    protected override void OnCast()
    {
        Vector2 forward = GetPlayerForward();
        Vector3 spawnPos = playerTransform.position + (Vector3)forward * 0.5f;

        GameObject proj = GameObject.Instantiate(activeData.prefab, spawnPos, Quaternion.identity);
        ProjectileMover projectileMover = proj.GetComponent<ProjectileMover>();

        if (projectileMover != null)
        {
            projectileMover.canPenetrate = true;
            projectileMover.Init(forward, activeData, 0, GetDamage());
        }
    }
}
