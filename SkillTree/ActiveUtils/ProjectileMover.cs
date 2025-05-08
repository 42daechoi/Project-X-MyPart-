using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    public float speed;
    public float maxDistance;
    public float damage;
    public bool canPenetrate = false;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    private Vector2 direction = Vector2.zero;
    private Vector3 startPosition;
    private bool isReady = false;


    public void Init(Vector2 dir, ActiveSkillData data, float chargeDuration, float damage)
    {
        direction = dir.normalized;
        startPosition = transform.position;
        speed = data.projectileSpeed;
        SetFlexData(data, chargeDuration);
        this.damage = damage;
        isReady = true;
    }

    private void SetFlexData(ActiveSkillData data, float chargeDuration)
    {
        float finalDamage = DamageCalculator.GetFinalAttackDamage(PlayerController.Instance.stats, damage);
        if (data.maxChargeDuration > 0)
        {
            if (chargeDuration < data.maxChargeDuration / 4)
            {
                damage = finalDamage / 4;
            }
            else
            {
                float chargingRatio = chargeDuration / data.maxChargeDuration;
                damage = finalDamage * chargingRatio;
            }
        }
        else
        {
            transform.localScale = data.projectileSize;
            damage = finalDamage;
        }
    }

    private void Update()
    {
        if (!isReady) return;

        transform.position += (Vector3)direction * speed * Time.deltaTime;

        float traveled = Vector2.Distance(new Vector2(startPosition.x, startPosition.y), new Vector2(transform.position.x, transform.position.y));
        if (traveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject target = collision.gameObject;

        if (target.CompareTag("Enemy"))
        {
            if (!canPenetrate)
            {
                //Damage(target);
                Destroy(gameObject);
            }
            else if (!hitTargets.Contains(target))
            {
                hitTargets.Add(target);
                //Damage(target);
            }
        }
        else if (target.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }


    //private void Damage(GameObject target)
    //{
    //    IDamageable dmg = target.GetComponent<IDamageable>();
    //    dmg?.TakeDamage(damage);
    //}

}
