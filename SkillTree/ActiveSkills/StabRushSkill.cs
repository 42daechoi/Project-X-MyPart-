using UnityEngine;
using DG.Tweening;

public class StabRushSkill : InstantActiveSkill
{
    public float dashDistanceMultiplier = 3f;
    public float dashDuration = 0.2f;
    public float hitboxWidth;
    public float hitboxHeight;
    public GameObject hitboxPrefab;

    protected override void OnCast()
    {
        Transform playerTransform = GetPlayerTransform();
        Collider2D playerCollider = playerTransform.GetComponent<Collider2D>();
        float playerWidth = playerCollider.bounds.size.x;
        float playerHeight = playerCollider.bounds.size.y;
        float dashDistance = playerWidth * dashDistanceMultiplier;

        Vector2 direction = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;
        Vector3 startPos = playerTransform.position;
        Vector3 dashEndPos = startPos + (Vector3)(direction * dashDistance);

        GameObject hitbox = Instantiate(hitboxPrefab, startPos, Quaternion.identity);
        SkillHitbox hitboxComponent = hitbox.GetComponent<SkillHitbox>();

        if (hitboxComponent != null)
        {
            hitboxComponent.size = new Vector2(playerWidth * 3, playerHeight);
            hitboxComponent.offset = new Vector2(playerWidth * 1.5f * Mathf.Sign(transform.localScale.x), 0);
            hitboxComponent.damage = DamageCalculator.GetFinalAttackDamage(PlayerController.Instance.stats, activeData.damageMultiplier);
            hitboxComponent.trackingTarget = playerTransform;
        }

        playerTransform.DOMove(dashEndPos, dashDuration).SetEase(Ease.Linear)
            .OnComplete(() => {
                Destroy(hitbox, 0.1f);
            });
    }
}