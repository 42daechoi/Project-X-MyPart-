using UnityEngine;
using DG.Tweening;

public class FireInchantSkill : InstantActiveSkill
{
    [SerializeField] private StatBinder statBinder;
    [SerializeField] private float duration = 5f;

    public FireInchantSkill(ActiveSkillData data) : base(data)
    {
    }

    protected override void OnCast()
    {
        statBinder = new StatBinder
        {
            type = StatType.AdditionalMeleePowerRate,
            value = 0.2f
        };
        PlayerController.Instance.stats = PlayerController.Instance.stats + statBinder;

        DOVirtual.DelayedCall(duration, () =>
        {
            PlayerController.Instance.stats = PlayerController.Instance.stats - statBinder;
        });
    }
}
