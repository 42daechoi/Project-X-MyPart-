using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ActiveSkillLoader
{
    private Dictionary<int, Func<ActiveSkillData, Skill>> registry = new()
    {
        { 1, data => new FireInchantSkill(data) },
        { 2, data => new FireBreathSkill(data) },
        { 3, data => new FireBallSkill(data) },
        { 4, data => new ElectricSpearSkill(data) },
        { 5, data => new ElectricPenetrateShotSkill(data) },
        { 6, data => new ElectricRushSkill(data) },
        { 7, data => new EarthRedutionShield(data) },
        { 8, data => new EarthDeflectSkill(data) },
        { 9, data => new EarthImmunity(data) },
    };

    public void LoadAllActiveSkills(Action<List<Skill>> onLoaded)
    {
        Addressables.LoadAssetsAsync<ActiveSkillData>("ActiveSkills", null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                List<Skill> loadedSkills = new();

                foreach (var data in handle.Result)
                {
                    if (registry.TryGetValue(data.id, out var constructor))
                    {
                        Skill skill = constructor(data);
                        loadedSkills.Add(skill);
                    }
                }

                Debug.Log($"ActiveSkillLoader: {loadedSkills.Count}개의 액티브 스킬 로딩 완료.");
                onLoaded?.Invoke(loadedSkills);
            }
            else
            {
                Debug.LogError("ActiveSkillLoader: 액티브 스킬 로딩 실패.");
                onLoaded?.Invoke(null);
            }
        };
    }
}
