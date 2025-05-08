using UnityEngine;
using DG.Tweening;

public class ElectricRushSkill : InstantActiveSkill
{
    public float dashDistanceMultiplier = 3f;
    public float dashDuration = 0.2f;
    public float hitboxWidth;
    public float hitboxHeight;

    public ElectricRushSkill(ActiveSkillData data) : base(data)
    {
    }

    protected override void OnCast()
    {
        Collider2D playerCollider = playerTransform.GetComponent<Collider2D>();
        float playerWidth = playerCollider.bounds.size.x;
        float playerHeight = playerCollider.bounds.size.y;
        float dashDistance = playerWidth * dashDistanceMultiplier;

        Vector2 direction = GetPlayerForward();
        Vector3 startPos = playerTransform.position;
        Vector3 dashEndPos = startPos + (Vector3)(direction * dashDistance);

        GameObject hitbox = GameObject.Instantiate(activeData.prefab, startPos, Quaternion.identity);
        SkillHitbox hitboxComponent = hitbox.GetComponent<SkillHitbox>();

        if (hitboxComponent != null)
        {
            hitboxComponent.size = new Vector2(playerWidth * 3, playerHeight);
            hitboxComponent.offset = new Vector2(playerWidth * 1.5f * GetPlayerForward().x, 0);
            hitboxComponent.damage = DamageCalculator.GetFinalAttackDamage(PlayerController.Instance.stats, GetDamage());
            hitboxComponent.trackingTarget = playerTransform;
        }

        playerTransform.DOMove(dashEndPos, dashDuration).SetEase(Ease.Linear)
            .OnComplete(() => {
                GameObject.Destroy(hitbox, 0.1f);
            });
    }
}