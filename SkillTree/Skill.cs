using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public SkillData data;

    public abstract void Activate();
}
