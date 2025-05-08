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

                Debug.Log("PassiveSkillLoader : 모든 패시브 스킬 로딩 완료.");
                onLoaded?.Invoke(loadedSkills);
            }
            else
            {
                Debug.LogError("PassiveSkillLoader : 패시브 스킬 로딩 실패.");
                onLoaded?.Invoke(null);
            }
        };
    }
}
