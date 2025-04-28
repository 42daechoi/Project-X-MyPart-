using UnityEngine;

public class ChargingIceSpearSkill : ChargingActiveSkill
{
    public GameObject projectilePrefab;
    private GameObject chargedProjectile;
    private ProjectileMover projectileMover;
    private Vector3 spawnPosition;
    private Vector2 direction;
    private Vector3 minScale;

    public override void OnChargeStart()
    {
        if (!CanStartCharge()) return;

        Transform playerTransform = GetPlayerTransform();
        if (playerTransform == null) return;

        UpdateProjectileDirectionAndPosition(playerTransform);

        chargedProjectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.Euler(0f, 0f, GetAngleFromDirection(direction)));
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

        Transform playerTransform = GetPlayerTransform();
        if (playerTransform != null)
        {
            UpdateProjectileDirectionAndPosition(playerTransform);
            chargedProjectile.transform.position = spawnPosition;
            chargedProjectile.transform.rotation = Quaternion.Euler(0f, 0f, GetAngleFromDirection(direction));
        }
    }

    public override void OnChargeCancelled()
    {
        if (chargedProjectile != null)
        {
            Destroy(chargedProjectile);
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
            projectileMover.Init(direction, activeData, chargeDuration);
        }
        chargedProjectile = null;
        return true;
    }

    private void UpdateProjectileDirectionAndPosition(Transform playerTransform)
    {
        bool isGravityInverted = playerTransform.up.y < 0;

        spawnPosition = playerTransform.position + Vector3.up * 1.5f;
        if (isGravityInverted)
        {
            spawnPosition.y = playerTransform.position.y - 1.5f;
        }
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        direction = (mouseWorldPosition - spawnPosition).normalized;
    }


    private float GetAngleFromDirection(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
