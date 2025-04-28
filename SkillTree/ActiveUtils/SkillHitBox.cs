using System.Collections.Generic;
using UnityEngine;

public class SkillHitbox : MonoBehaviour
{
    public float damage;
    public Vector2 overlapPosition;
    public Vector2 size;
    public Transform trackingTarget;
    public Vector2 offset;

    private List<Collider2D> alreadyHitColliders = new List<Collider2D>();

    private void FixedUpdate()
    {
        if (trackingTarget != null)
        {
            overlapPosition = trackingTarget.position + (Vector3)offset;
        }

        ApplyDamage();
    }

    private void ApplyDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(overlapPosition, size, 0f, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            if (!alreadyHitColliders.Contains(hit))
            {
                Debug.Log($"{hit.gameObject.name}에게 데미지 {damage} 입힘");
                //var enemy = hit.GetComponent<Enemy>();
                //if (enemy != null) enemy.TakeDamage(damage);
                alreadyHitColliders.Add(hit);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(overlapPosition, size);
    }
}