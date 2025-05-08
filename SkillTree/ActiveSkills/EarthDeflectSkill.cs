using DG.Tweening;
using UnityEngine;

public class EarthDeflectSkill : InstantActiveSkill
{
    private float duration = 5f;
    public float offset = 1f;

    public EarthDeflectSkill(ActiveSkillData data) : base(data) { }

    protected override void OnCast()
    {
        GameObject shieldObj = GameObject.Instantiate(activeData.prefab, playerTransform);
        shieldObj.transform.position = playerTransform.position + (Vector3)(GetPlayerForward() * offset);
        shieldObj.GetComponent<BoxCollider2D>().enabled = true;

        DOVirtual.DelayedCall(duration, () =>
        {
            GameObject.Destroy(shieldObj);
        });
    }
}
