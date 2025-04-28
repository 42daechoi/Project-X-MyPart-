using UnityEngine;

public class PenetrateArrowSkill : InstantActiveSkill
{
    public GameObject projectilePrefab;

    protected override void OnCast()
    {
        Transform playerTransform = GetPlayerTransform();
        Vector2 forward = playerTransform.localScale.x > 0 ? Vector2.right : Vector2.left;

        Vector3 spawnPos = playerTransform.position + (Vector3)forward * 0.5f;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        ProjectileMover projectileMover = proj.GetComponent<ProjectileMover>();

        if (projectileMover != null)
        {
            projectileMover.canPenetrate = true;
            projectileMover.Init(forward, activeData, 0);
        }
    }
}
