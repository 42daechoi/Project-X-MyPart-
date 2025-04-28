using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkillData", menuName = "SkillData/ActiveSkillData")]
public class ActiveSkillData : SkillData
{
    [Header("��Ƽ�� ��ų �ʼ� ����")]
    public List<StatBinder> cost;
    public float cooldown;
    public float damageMultiplier;
    public float projectileSpeed;
    public Vector3 projectileSize;

    [Header("��Ÿ� ���� �ɼ�")]
    public float maxDistance;

    [Header("��¡ �ɼ�")]
    public float maxChargeDuration;
}
