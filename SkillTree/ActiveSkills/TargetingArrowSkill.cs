using UnityEngine;

public class TargetingArrowSkill : InstantActiveSkill
{
    public float range = 10f;
    public int maxHits = 4;

    public TargetingArrowSkill(ActiveSkillData data) : base(data) { }

    protected override void OnCast()
    {
        Vector2 forward = playerTransform.localScale.x > 0 ? Vector2.right : Vector2.left;

        Vector3 spawnPos = playerTransform.position + (Vector3)forward * 0.5f;

        GameObject proj = GameObject.Instantiate(activeData.prefab, spawnPos, Quaternion.identity);
        TargetingProjectile targetingArrow = proj.GetComponent<TargetingProjectile>();

        if (targetingArrow != null)
        {
            targetingArrow.Init(forward, activeData, range, maxHits, GetDamage());
        }
    }
}