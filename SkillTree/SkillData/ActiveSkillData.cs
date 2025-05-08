using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkillData", menuName = "SkillData/ActiveSkillData")]
public class ActiveSkillData : SkillData
{
    [Header("액티브 스킬 필수 설정")]
    public StatBinder cost;
    public float[] damage;
    public int upgradeLevel;
    public GameObject prefab;

    [Header("투사체 옵션")]
    public float projectileSpeed;
    public Vector3 projectileSize;

    [Header("사거리 제한 옵션")]
    public float maxDistance;

    [Header("차징 옵션")]
    public float maxChargeDuration;
    public GameObject targetPositionPrefab;
}
