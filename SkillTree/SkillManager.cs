using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    private PassiveSkillLoader passiveLoader = new();
    private ActiveSkillLoader activeLoader = new();

    [Header("Debug Only")]
    [SerializeField] private List<SerializableSkillView> skillViewList = new();

    public List<Skill> allSkills = new();
    public List<Skill> unlockedActiveSkills = new();
    public List<Skill> unlockedPassiveSkills = new();
    private PlayerController playerController;

    private int currentSkillIndex = 0;
    private float rightClickStartTime = 0f;
    private bool isCharge = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        passiveLoader.LoadAllPassiveSkills(OnSkillsLoaded);
        activeLoader.LoadAllActiveSkills(OnSkillsLoaded);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ChargeStartOrActivate();
        }
        if (isCharge && unlockedActiveSkills.Count > 0)
        {
            Charging();
        }
        if (Input.GetMouseButtonUp(1) && isCharge)
        {
            ActivateChargedSkill();
        }
    }

    private void UpdateSkillViewList()
    {
        skillViewList.Clear();
        foreach (var skill in unlockedActiveSkills)
        {
            skillViewList.Add(new SerializableSkillView
            {
                id = skill.data.id,
                name = skill.data.skillName,
                type = skill is ActiveSkill ? "Active" : "Passive"
            });
        }
    }

    private void OnSkillsLoaded(List<Skill> list)
    {
        if (list == null)
        {
            Debug.LogError("SkillManager : 스킬 로딩 실패.");
            return;
        }
        allSkills.AddRange(list);
        UpdateSkillViewList();
        Debug.Log($"SkillManager : {list.Count}개의 스킬이 추가됨.");
    }

    private void Charging()
    {
        if (unlockedActiveSkills[currentSkillIndex] is ChargingActiveSkill chargingSkill)
        {
            float currentChargeDuration = Time.time - rightClickStartTime;
            chargingSkill.OnCharging(currentChargeDuration);
        }
    }

    private void ChargeStartOrActivate()
    {
        if (unlockedActiveSkills.Count > 0)
        {
            Debug.Log("Current skill: " + unlockedActiveSkills[currentSkillIndex].GetType().Name);
            if (unlockedActiveSkills[currentSkillIndex] is ChargingActiveSkill chargingSkill)
            {
                rightClickStartTime = Time.time;
                isCharge = true;
                chargingSkill.OnChargeStart();
            }
            else
            {
                unlockedActiveSkills[currentSkillIndex].Activate();
                Debug.Log($"SkillManager : {unlockedActiveSkills[currentSkillIndex].data.skillName} 스킬 사용");
                MoveToNextSkill();
            }
        }
    }

    private void ActivateChargedSkill()
    {
        isCharge = false;
        if (unlockedActiveSkills.Count == 0)
            return;

        float chargeDuration = Time.time - rightClickStartTime;

        if (unlockedActiveSkills[currentSkillIndex] is ChargingActiveSkill chargingSkill)
        {
            chargingSkill.ActivateWithCharge(chargeDuration);
            Debug.Log($"SkillManager : {chargingSkill.activeData.skillName} 스킬 사용 ({chargeDuration:F2}초 차징)");
        }
        MoveToNextSkill();
    }

    private void MoveToNextSkill()
    {
        currentSkillIndex = (currentSkillIndex + 1) % unlockedActiveSkills.Count;
    }

    public void UnlockSkill(SkillData skillData)
    {
        if (skillData == null)
        {
            Debug.LogError("SkillManager : 해금하려는 스킬 데이터가 null 값입니다.");
            return;
        }
        Skill skill = GetSkillById(skillData.id);
        List<Skill> targetList = GetTargetList(skill);
        UpgradeSkillLevel(targetList, skill);
        if (targetList != null && !targetList.Contains(skill))
        {
            targetList.Add(skill);
            ApplyPassiveStats(skill, true);
            Debug.Log($"SkillManager : {skill.data.skillName} 스킬 해금");
            UpdateSkillViewList();
        }
    }

    private void UpgradeSkillLevel(List<Skill> targetList, Skill skill)
    {
        if (!targetList.Contains(skill)) return;
        if (skill is ActiveSkill activeSkill)
        {
            activeSkill.UpgradeSkill();
            Debug.Log($"SkillManager : {activeSkill.data.name}의 레벨 : {activeSkill.GetUpgradeLevel()}");
        }
    }

    private Skill GetSkillById(int id)
    {
        foreach (Skill skill in allSkills)
        {
            if (skill.data.id == id) return skill;
        }
        return null;
    }

    private void ApplyPassiveStats(Skill skill, bool isActivate)
    {
        if (skill is PassiveSkill passiveSkill)
        {
            if (isActivate) passiveSkill.Activate();
            else passiveSkill.Deactivate();
        }
    }

    public void LockSkill(SkillData skillData)
    {
        if (skillData == null)
        {
            Debug.LogError("SkillManager : 잠그려는 스킬의 데이터가 null 값입니다.");
            return;
        }
        Skill skill = GetSkillById(skillData.id);
        List<Skill> targetList = GetTargetList(skill);
        if (targetList != null && targetList.Contains(skill))
        {
            if (canDowngradeSkillLevel(targetList, skill) == false)
            {
                ApplyPassiveStats(skill, false);
                targetList.Remove(skill);
                Debug.Log($"SkillManager : {skill.data.skillName} 스킬 잠금");
                UpdateSkillViewList();
            }
        }
    }

    private bool canDowngradeSkillLevel(List<Skill> targetList, Skill skill)
    {
        if (skill is ActiveSkill activeSkill)
        {
            if (activeSkill.GetUpgradeLevel() > 0)
            {
                activeSkill.DowngradeSkill();
                Debug.Log($"SkillManager : {activeSkill.data.name}의 레벨 : {activeSkill.GetUpgradeLevel()}");
                return true;
            }
        }
        return false;
    }

    private List<Skill> GetTargetList(Skill skill)
    {
        if (skill is ActiveSkill)
            return unlockedActiveSkills;
        else if (skill is PassiveSkill)
            return unlockedPassiveSkills;

        return null;
    }
}
