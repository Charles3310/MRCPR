using System.Collections.Generic;
using UnityEngine;

public class GoodPool : MonoBehaviour
{

    [SerializeField] private GameObject goodPrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();


    public GameObject Get()
    {
        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(goodPrefab, this.gameObject.transform);
            obj.GetComponent<Good>().used += Return;
            pool.Enqueue(obj);
        }

        GameObject go = pool.Dequeue();
        go.SetActive(true);
        return go;
    }

    private void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
