using UnityEngine;
using System.Collections.Generic;

public class SkillData : ScriptableObject
{
    [Header("��ų ���� �ʼ� ����")]
    public int id;
    public string skillName;
    [TextArea] public string description;
    public ElementType type;
}