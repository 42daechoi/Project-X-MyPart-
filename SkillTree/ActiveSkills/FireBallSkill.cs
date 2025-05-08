using UnityEngine;

public class FireBallSkill : ChargingActiveSkill
{
    private GameObject chargedProjectile;
    private ProjectileMover projectileMover;
    private Vector3 spawnPosition;
    private Vector3 targetPosition;
    private Vector3 minScale;
    private float offset = 4f;

    public LayerMask groundLayer;

    public FireBallSkill(ActiveSkillData data) : base(data)
    {
    }

    public override void OnChargeStart()
    {
        if (playerTransform == null) return;
        UpdateProjectilePositions(playerTransform);

        chargedProjectile = GameObject.Instantiate(
            activeData.prefab,
            spawnPosition,
            Quaternion.Euler(0f, 0f, GetAngleFromDirection(targetPosition - spawnPosition))
        );

        projectileMover = chargedProjectile.GetComponent<ProjectileMover>();
        if (projectileMover != null) projectileMover.enabled = false;

        minScale = activeData.projectileSize * 0.25f;
        chargedProjectile.transform.localScale = minScale;
    }

    public override void OnCharging(float chargeDuration)
    {
        if (chargedProjectile == null) return;

        float chargeRatio = Mathf.Clamp01(chargeDuration / activeData.maxChargeDuration);
        Vector3 newScale = Vector3.Lerp(minScale, activeData.projectileSize, chargeRatio);
        chargedProjectile.transform.localScale = newScale;
    }

    public override void OnChargeCancelled()
    {
        if (chargedProjectile != null)
        {
            GameObject.Destroy(chargedProjectile);
            chargedProjectile = null;
        }
    }

    protected override bool OnCastCharged(float chargeDuration)
    {
        if (chargedProjectile == null) return false;

        float chargeRatio = Mathf.Clamp01(chargeDuration / activeData.maxChargeDuration);
        Vector3 finalScale = Vector3.Max(minScale, Vector3.Lerp(minScale, activeData.projectileSize, chargeRatio));
        chargedProjectile.transform.localScale = finalScale;

        if (projectileMover != null)
        {
            projectileMover.enabled = true;
            Vector2 direction = (targetPosition - spawnPosition).normalized;
            projectileMover.Init(direction, activeData, chargeDuration, GetDamage());
        }

        chargedProjectile = null;
        return true;
    }

    private void UpdateProjectilePositions(Transform playerTransform)
    {
        Vector3 upDirection = playerTransform.up.normalized;
        spawnPosition = playerTransform.position + upDirection * 2f;

        Vector2 forward = GetPlayerForward();
        forward *= offset;
        Vector2 rayOrigin = (Vector2)spawnPosition + forward;

        Vector2 rayDirection = -playerTransform.up;
        Debug.Log($"{rayOrigin}, {rayDirection}");
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 10f, groundLayer);
        if (hit.collider != null)
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = rayOrigin + rayDirection * 3f;
        }
        if (activeData.targetPositionPrefab != null)
        {
            GameObject marker = GameObject.Instantiate(activeData.targetPositionPrefab, targetPosition, Quaternion.identity);
            GameObject.Destroy(marker, 1f);
        }
    }


    private float GetAngleFromDirection(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
