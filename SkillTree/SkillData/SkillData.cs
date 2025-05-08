using UnityEngine;
using System.Collections.Generic;

public class SkillData : ScriptableObject
{
    [Header("스킬 공통 필수 설정")]
    public int id;
    public string skillName;
    [TextArea] public string description;
    public ElementType type;
}