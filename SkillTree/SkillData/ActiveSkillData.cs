using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkillData", menuName = "SkillData/ActiveSkillData")]
public class ActiveSkillData : SkillData
{
    [Header("��Ƽ�� ��ų �ʼ� ����")]
    public StatBinder cost;
    public float[] damage;
    public int upgradeLevel;
    public GameObject prefab;

    [Header("����ü �ɼ�")]
    public float projectileSpeed;
    public Vector3 projectileSize;

    [Header("��Ÿ� ���� �ɼ�")]
    public float maxDistance;

    [Header("��¡ �ɼ�")]
    public float maxChargeDuration;
    public GameObject targetPositionPrefab;
}
