using System.Collections.Generic;
using UnityEngine;

public class ItemUIPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject itemUIPrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();

    public GameObject Get()
    {
        GameObject go;
        if (pool.Count > 0)
        {
            go = pool.Dequeue();
        }
        else
        {
            go = Instantiate(itemUIPrefab);
        }

        go.transform.SetParent(null);
        go.SetActive(true);
        return go;
    }

    public void Return(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(this.transform);
        pool.Enqueue(go);
    }
}
