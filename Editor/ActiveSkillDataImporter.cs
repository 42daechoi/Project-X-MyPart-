//#if UNITY_EDITOR
//using UnityEngine;
//using UnityEditor;
//using System;
//using System.Collections.Generic;
//using System.IO;

//public class ActiveSkillDataImporter : EditorWindow
//{
//    [Serializable]
//    public class SkillJsonList
//    {
//        public List<SkillJson> list;
//    }

//    [Serializable]
//    public class SkillJson
//    {
//        public int id;
//        public string skillName;
//        public string description;
//        public string weaponType;
//        public string prerequisitesAnd;
//        public string prerequisitesOr;
//        public string costType;
//        public string costValue;
//        public float cooldown;
//        public float damageMultiplier;
//        public float projectileSpeed;
//        public float maxDistance;
//        public float maxChargeDuration;
//    }

//    [MenuItem("Tools/JsonToSO")]
//    public static void ImportSkillsFromJson()
//    {
//        string jsonPath = Application.dataPath + "/ScriptableObjects/Skills/ActiveSkills/Skill_data.json";
//        string skillFolderPath = "Assets/ScriptableObjects/Skills/ActiveSkills";

//        if (!File.Exists(jsonPath) || !Directory.Exists(skillFolderPath))
//        {
//            Debug.LogError("ActiveSkillDataImporter: 경로를 찾을 수 없거나 json을 찾을 수 없습니다.");
//            return;
//        }

//        DeleteAllSOInPath(skillFolderPath);

//        string json = File.ReadAllText(jsonPath);
//        string wrappedJson = "{\"list\":" + json + "}";
//        SkillJsonList parsedList = JsonUtility.FromJson<SkillJsonList>(wrappedJson);

//        foreach (var item in parsedList.list)
//        {
//            ActiveSkillData skill = ScriptableObject.CreateInstance<ActiveSkillData>();
//            InitProps(item, skill);
//            ParsePrerequisitesAnd(item, skill);
//            ParsePrerequisitesOr(item, skill);
//            ParseCost(item, skill);

//            string assetPath = $"{skillFolderPath}/{skill.skillName}.asset";
//            AssetDatabase.CreateAsset(skill, assetPath);
//        }
//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();

//        Debug.Log("ActiveSkillDataImporter: Skill 데이터 가져오기 완료.");
//    }

//    private static void DeleteAllSOInPath(string skillFolderPath)
//    {
//        var guids = AssetDatabase.FindAssets("t:SkillData", new[] { skillFolderPath });
//        foreach (var guid in guids)
//        {
//            string path = AssetDatabase.GUIDToAssetPath(guid);
//            AssetDatabase.DeleteAsset(path);
//        }
//    }

//    private static void InitProps(SkillJson item, ActiveSkillData skill)
//    {
//        skill.id = item.id;
//        skill.skillName = item.skillName;
//        skill.description = item.description;
//        skill.type = (ElementType)Enum.Parse(typeof(ElementType), item.weaponType);
//        skill.damageMultiplier = item.damageMultiplier;
//        skill.projectileSpeed = item.projectileSpeed;
//        skill.maxDistance = item.maxDistance;
//        skill.maxChargeDuration = item.maxChargeDuration;
//        skill.cost = item.cost;
//        skill.projectileSize = new Vector3(1, 1, 1);
//        skill.cooldown = item.cooldown;
//        skill.prerequisitesAnd = new List<int>();
//        skill.prerequisitesOr = new List<int>();
//    }

//    private static void ParseCost(SkillJson item, ActiveSkillData skill)
//    {
//        if (!string.IsNullOrEmpty(item.costType) && !string.IsNullOrEmpty(item.costValue))
//        {
//            string[] types = item.costType.Split('/');
//            string[] values = item.costValue.Split('/');

//            for (int i = 0; i < types.Length && i < values.Length; i++)
//            {
//                string trimmedType = types[i].Trim();
//                string trimmedValue = values[i].Trim();

//                if (Enum.TryParse(trimmedType, out StatType statType) && float.TryParse(trimmedValue, out float val))
//                {
//                    skill.cost.Add(new StatBinder { type = statType, value = val });
//                }
//            }
//        }
//    }

//    private static void ParsePrerequisitesAnd(SkillJson item, ActiveSkillData skill)
//    {
//        if (!string.IsNullOrEmpty(item.prerequisitesAnd))
//        {
//            string[] prereqIds = item.prerequisitesAnd.Split('/');
//            foreach (var idStr in prereqIds)
//            {
//                if (int.TryParse(idStr.Trim(), out int id))
//                {
//                    skill.prerequisitesAnd.Add(id);
//                }
//            }
//        }
//    }

//    private static void ParsePrerequisitesOr(SkillJson item, ActiveSkillData skill)
//    {
//        if (!string.IsNullOrEmpty(item.prerequisitesOr))
//        {
//            string[] prereqIds = item.prerequisitesOr.Split('/');
//            foreach (var idStr in prereqIds)
//            {
//                if (int.TryParse(idStr.Trim(), out int id))
//                {
//                    skill.prerequisitesOr.Add(id);
//                }
//            }
//        }
//    }
//}
//#endif
