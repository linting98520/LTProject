using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Moving : MonoBehaviour
{
    public bool StartMove = false;
    public float Speed = 5.0f;
    public Vector3 Movement = Vector3.zero;

    [Button]
    public void Change()
    {
        StartMove = !StartMove;
        if (StartMove)
        {
            transform.position += Movement;
        }
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Movement, Time.deltaTime * Speed);
    }
}
