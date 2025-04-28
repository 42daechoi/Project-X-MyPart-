using UnityEngine;

public class SummonAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float attackInterval = 1f;
    private float lastAttackTime = 0f;
    private ActiveSkillData data;
    private bool isReady = false;

    void Update()
    {
        if (Time.time - lastAttackTime >= attackInterval)
        {
            lastAttackTime = Time.time;
            FireProjectile();
        }
    }

    public void SetData(ActiveSkillData activeData)
    {
        data = activeData;
        isReady = true;
    }

    private void FireProjectile()
    {
        if (!isReady) return;

        Transform parent = transform.parent;
        if (parent == null) return;

        float xScale = parent.localScale.x;
        Vector2 direction = xScale >= 0 ? Vector2.right : Vector2.left;

        bool isGravityInverted = parent.up.y < 0;
        if (isGravityInverted)
            direction *= -1;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        ProjectileMover projectileMover = proj.GetComponent<ProjectileMover>();
        projectileMover?.Init(direction, data, 0);
    }
}
