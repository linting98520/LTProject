using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomethingCaculate : MonoBehaviour
{
    public void VectorCross(Vector2 p, Vector2 p1, Vector2 p2)
    {
        var res = (p1.x - p.x) * (p2.x - p.x) - (p1.y - p.y) * (p2.y - p.y);
        Debug.Log(res);
    }
}
