using UnityEngine;

public class TargetingArrowSkill : InstantActiveSkill
{
    public GameObject projectilePrefab;
    public float range = 10f;
    public int maxHits = 4;

    protected override void OnCast()
    {
        Transform playerTransform = GetPlayerTransform();
        Vector2 forward = playerTransform.localScale.x > 0 ? Vector2.right : Vector2.left;

        Vector3 spawnPos = playerTransform.position + (Vector3)forward * 0.5f;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        TargetingProjectile targetingArrow = proj.GetComponent<TargetingProjectile>();

        if (targetingArrow != null)
        {
            targetingArrow.Init(forward, activeData, range, maxHits);
        }
    }
}