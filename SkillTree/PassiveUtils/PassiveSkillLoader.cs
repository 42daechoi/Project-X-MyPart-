using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System;

public class PassiveSkillLoader
{
    public void LoadAllPassiveSkills(Action<List<Skill>> onLoaded)
    {
        Addressables.LoadAssetsAsync<PassiveSkillData>("PassiveSkills", null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                List<Skill> loadedSkills = new();

                foreach (var data in handle.Result)
                {
                    PassiveSkill skill = new PassiveSkill(data);
                    loadedSkills.Add(skill);
                }

                Debug.Log("PassiveSkillLoader : ��� �нú� ��ų �ε� �Ϸ�.");
                onLoaded?.Invoke(loadedSkills);
            }
            else
            {
                Debug.LogError("PassiveSkillLoader : �нú� ��ų �ε� ����.");
                onLoaded?.Invoke(null);
            }
        };
    }
}
