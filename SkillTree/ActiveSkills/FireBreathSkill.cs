using DG.Tweening;
using UnityEngine;

public class FireBreathSkill : InstantActiveSkill
{
    public float hitboxWidth;
    public float hitboxHeight;

    public FireBreathSkill(ActiveSkillData data) : base(data)
    {
    }

    protected override void OnCast()
    {
        Collider2D playerCollider = playerTransform.GetComponent<Collider2D>();
        float playerWidth = playerCollider.bounds.size.x * 3;
        float playerHeight = playerCollider.bounds.size.y * 2;

        Vector2 direction = GetPlayerForward();
        Vector3 startPos = playerTransform.position;
        Vector3 dashEndPos = startPos + (Vector3)(direction);

        GameObject hitbox = GameObject.Instantiate(activeData.prefab, startPos, Quaternion.identity);
        SkillHitbox hitboxComponent = hitbox.GetComponent<SkillHitbox>();

        if (hitboxComponent != null)
        {
            hitboxComponent.size = new Vector2(playerWidth * 3, playerHeight);
            hitboxComponent.offset = new Vector2(playerWidth * 1.5f * GetPlayerForward().x, 0);
            hitboxComponent.damage = DamageCalculator.GetFinalAttackDamage(PlayerController.Instance.stats, GetDamage());
            hitboxComponent.trackingTarget = playerTransform;
        }
        DOVirtual.DelayedCall(1f, () =>
            {
                GameObject.Destroy(hitbox);
            });
    }
}
