using DG.Tweening;
using UnityEngine;

public class EarthImmunity : InstantActiveSkill
{
    private float duration = 5f;

    public EarthImmunity(ActiveSkillData data) : base(data) { }

    protected override void OnCast()
    {
        GameObject shieldObj = GameObject.Instantiate(activeData.prefab, playerTransform);
        shieldObj.transform.position = playerTransform.position;
        BoxCollider2D shieldCollider = shieldObj.GetComponent<BoxCollider2D>();
        shieldCollider.enabled = true;
        Vector2 originalSize = shieldCollider.size;
        shieldCollider.size = new Vector2(originalSize.x * 2, originalSize.y * 2);

        DOVirtual.DelayedCall(duration, () =>
        {
            GameObject.Destroy(shieldObj);
        });
    }
}
