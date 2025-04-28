using UnityEngine;

public class SummonSkill : InstantActiveSkill
{
    public GameObject summonPrefab;
    public float summonDuration = 5f;

    protected override void OnCast()
    {
        Transform playerTransform = GetPlayerTransform();

        Vector3 summonOffset = playerTransform.right * -0.5f + playerTransform.up * 1.5f;
        Vector3 summonPosition = playerTransform.position + summonOffset;

        GameObject summonObj = Instantiate(summonPrefab, summonPosition, Quaternion.identity);
        summonObj.transform.SetParent(playerTransform);
        summonObj.GetComponent<SummonAttack>()?.SetData(activeData);

        Destroy(summonObj, summonDuration);
    }

}
