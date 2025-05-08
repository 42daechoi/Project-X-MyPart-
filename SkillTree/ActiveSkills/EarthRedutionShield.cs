using UnityEngine;
using DG.Tweening;

public class EarthRedutionShield : InstantActiveSkill
{
    [SerializeField] private StatBinder statBinder;
    private float duration = 5f;

    public EarthRedutionShield(ActiveSkillData data) : base(data) { }

    protected override void OnCast()
    {
        statBinder = new StatBinder
        {
            type = StatType.AdditionalDefensePowerRate,
            value = 0.15f
        };
        PlayerController.Instance.stats = PlayerController.Instance.stats + statBinder;
        GameObject shieldObj = GameObject.Instantiate(activeData.prefab, playerTransform);
        shieldObj.transform.position = playerTransform.position;

        DOVirtual.DelayedCall(duration, () =>
        {
            PlayerController.Instance.stats = PlayerController.Instance.stats - statBinder;
            GameObject.Destroy(shieldObj);
        });
    }
}
