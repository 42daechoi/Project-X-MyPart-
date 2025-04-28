using UnityEngine;

public class IceSpearSkill : InstantActiveSkill
{
    public GameObject projectilePrefab;
    public int count = 3;
    public float spacing = 0.5f;

    protected override void OnCast()
    {
        Transform playerTransform = GetPlayerTransform();
        Vector2 forward = playerTransform.localScale.x > 0 ? Vector2.right : Vector2.left;
        bool isGravityInverted = playerTransform.up.y < 0;
        if (isGravityInverted)
        {
            forward *= -1;
        }

        Vector2 right = new Vector2(forward.y, -forward.x);

        int mid = count / 2;

        for (int i = 0; i < count; i++)
        {
            float offset = (i - mid) * spacing;

            Vector3 spawnPos = playerTransform.position
                             + (Vector3)(right * offset)
                             + (Vector3)(playerTransform.up * 1.5f);

            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            ProjectileMover projectileMover = proj.GetComponent<ProjectileMover>();
            projectileMover?.Init(forward, activeData, 0);
        }
    }
}
