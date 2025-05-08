using UnityEngine;

public class ElectricSpearSkill : InstantActiveSkill
{
	public float offset = 1f;

    public ElectricSpearSkill(ActiveSkillData data) : base(data)
    {
    }

    protected override void OnCast()
	{
		Vector2 forward = GetPlayerForward();
		Vector3 spawnPos = playerTransform.position + (Vector3)(GetPlayerForward() * offset);

		GameObject proj = GameObject.Instantiate(activeData.prefab, spawnPos, Quaternion.identity);
		ProjectileMover projectileMover = proj.GetComponent<ProjectileMover>();
		projectileMover?.Init(forward, activeData, 0, GetDamage());
	}
}
