using DG.Tweening;
using UnityEngine;

public class TargetingProjectile : MonoBehaviour
{
	public float speed = 10f;
	public float range;
	public int maxHits = 4;
	public float randomDirTime = 0.1f;
	private Vector2 direction;
	private float damage;
    private int hitCount = 0;
	private bool isHit = false;
	private GameObject hitTarget;
    private Tween tween;


    private void Update()
	{
		transform.position += (Vector3)direction * speed * Time.deltaTime;
	}

	public void Init(Vector2 dir, ActiveSkillData data, float trackingRange, int hits, float damage)
	{
		direction = dir.normalized;
		range = trackingRange;
		maxHits = hits;
		this.damage = DamageCalculator.GetFinalAttackDamage(PlayerController.Instance.stats, damage);

        DOVirtual.DelayedCall(10f, () =>
        {
            if (this != null && gameObject != null && gameObject.activeInHierarchy)
            {
                Destroy(gameObject);
            }
        });
    }

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("Enemy"))
		{
			if (!isHit) isHit = true;
			if (hitTarget == collision.gameObject) return;
			hitCount++;
            hitTarget = collision.gameObject;
            // IDamageable dmg = collision.GetComponent<IDamageable>();
            // dmg?.TakeDamage(damage);
            if (hitCount >= maxHits)
            {
                Destroy(gameObject);
                return;
            }
            MoveToTarget();

		}
		else
		{
			if (!isHit) Destroy(gameObject);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Enemy"))
		{
			hitTarget = null;
		}
    }

    private void MoveToTarget()
	{
		Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask("Enemy"));
		if (enemies.Length == 1)
		{
			RandomDirAndReturn();
            return;
		}
		GameObject target = FindTargetWithoutHitTarget(enemies, hitTarget);
        if (target == null)
		{
			return;
		}
		direction = (target.transform.position - transform.position).normalized;
    }

	private GameObject FindTargetWithoutHitTarget(Collider2D[] enemies, GameObject hitTarget)
	{
		GameObject closest = null;
		float minDist = Mathf.Infinity;

		foreach (var e in enemies)
		{
			if (e == hitTarget.GetComponent<Collider2D>()) continue;
			float dist = Vector2.Distance(transform.position, e.transform.position);
			if (dist < minDist)
			{
				minDist = dist;
				closest = e.gameObject;
			}
		}

		return closest;
	}

    private void RandomDirAndReturn()
    {
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 1f)).normalized;
        direction = randomDirection;
        Debug.Log(direction);

        tween = DOVirtual.DelayedCall(randomDirTime, () =>
        {
            direction = -randomDirection;
            Debug.Log(direction);
        });
    }
}
