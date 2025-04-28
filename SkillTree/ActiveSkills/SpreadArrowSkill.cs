using UnityEngine;

public class SpreadArrowSkill : InstantActiveSkill
{
    public GameObject projectilePrefab;
    public float angleOffset = 15f;

    protected override void OnCast()
    {
        Transform playerTransform = GetPlayerTransform();
        Vector2 forward = playerTransform.localScale.x > 0 ? Vector2.right : Vector2.left;

        float[] angles = { 0f, angleOffset, -angleOffset };

        foreach (float angle in angles)
        {
            Vector2 direction = Rotate(forward, angle * Mathf.Deg2Rad);
            Vector3 spawnPos = playerTransform.position + (Vector3)direction.normalized * 0.5f;

            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            ProjectileMover projectileMover = proj.GetComponent<ProjectileMover>();
            projectileMover?.Init(direction, activeData, 0);
        }
    }

    private Vector2 Rotate(Vector2 v, float radians)
    {
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }
}
