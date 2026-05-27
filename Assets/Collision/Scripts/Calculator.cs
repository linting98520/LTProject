using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Calculator : MonoBehaviour
{
    [Button]
    public void CalCross(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        Vector2 r = a2 - a1;
        Vector2 s = b2 - b1;

        //計算叉積，公式 x1 * y2 - y1 * x2
        //結果不為 0，可以確定兩向量不平行，有機會相交
        float rs = (r.x * s.y) - (r.y * s.x);
        Debug.Log($"CalCross = {rs}");
    }

    [Button]
    public void GetIntersectPoint(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        Vector2 r = a2 - a1;
        Vector2 s = b2 - b1;

        //計算叉積，公式 x1 * y2 - y1 * x2
        //結果不為 0，可以確定兩向量不平行，有機會相交
        float rs = (r.x * s.y) - (r.y * s.x);

        if (Mathf.Abs(rs) < 1e-6f)
            Debug.Log($"GetIntersectPoint = 不相交");

        double t = ((double)((b1.x - a1.x) * s.y) - (double)((b1.y - a1.y) * s.x)) / rs; // ((B1 - A1) * s) / r * s
        double u = ((double)((b1.x - a1.x) * r.y) - (double)((b1.y - a1.y) * r.x)) / rs; // ((B1 - A1) * r) / r * s

        //交點如果在 (a2-a1) 和 (b2-b1) 的線段上，t 和 u 應該為大於等於0且小於等於1
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            Vector2 dv2 = r;
            dv2.x = (float)(t * dv2.x);
            dv2.y = (float)(t * dv2.y);

            var point = a1 + dv2; //a1 + (t * r)
            Debug.Log($"GetIntersectPoint = {point}");
        }
    }
}
