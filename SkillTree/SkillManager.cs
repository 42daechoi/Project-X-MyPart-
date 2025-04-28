using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
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
            if (unlockedActiveSkills[currentSkillIndex] is ChargingActiveSkill chargingSkill)
            {
                rightClickStartTime = Time.time;
                isCharge = true;
                chargingSkill.OnChargeStart();
            }
            else
            {
                unlockedActiveSkills[currentSkillIndex].Activate();
                Debug.Log($"SkillManager : {unlockedActiveSkills[currentSkillIndex].data.skillName} ��ų ���");
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
            Debug.Log($"SkillManager : {chargingSkill.activeData.skillName} ��ų ��� ({chargeDuration:F2}�� ��¡)");
        }
        MoveToNextSkill();
    }

    private void MoveToNextSkill()
    {
        currentSkillIndex = (currentSkillIndex + 1) % unlockedActiveSkills.Count;
    }

    public bool UnlockSkill(Skill skill)
    {
        if (skill == null)
        {
            Debug.LogError("SkillManager : �ر��Ϸ��� ��ų�� null ���Դϴ�.");
            return false;
        }

        List<Skill> targetList = GetTargetList(skill);

        if (targetList != null && !targetList.Contains(skill))
        {
            targetList.Add(skill);
            ApplyPassiveStats(skill, true);
            Debug.Log($"SkillManager : {skill.data.skillName} ��ų �ر�");
            return true;
        }
        return false;
    }

    private void ApplyPassiveStats(Skill skill, bool isActivate)
    {
        if (skill is PassiveSkill passiveSkill)
        {
            if (isActivate) passiveSkill.Activate();
            else passiveSkill.Deactivate();
        }
    }

    public bool LockSkill(Skill skill)
    {
        if (skill == null)
        {
            Debug.LogError("SkillManager : ��׷��� ��ų�� null ���Դϴ�.");
            return false;
        }

        List<Skill> targetList = GetTargetList(skill);

        if (targetList != null && targetList.Contains(skill))
        {
            ApplyPassiveStats(skill, false);
            targetList.Remove(skill);
            Debug.Log($"SkillManager : {skill.data.skillName} ��ų ���");
            return true;
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
