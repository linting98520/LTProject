using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class LeanPoolTest : MonoBehaviour
{
    public LeanGameObjectPool pool;
    public Transform Root;
    private Queue<GameObject> objs = new Queue<GameObject>();

    private void OnEnable()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject obj = null;
            if (pool.TrySpawn(ref obj, Root))
            {
                objs.Enqueue(obj);
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (objs.Count > 0)
            {
                GameObject obj = objs.Dequeue();
                pool.Despawn(obj);
            }
        }
    }
}
