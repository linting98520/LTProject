using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CollisionPointTool
{
    private static List<Vector2> atkVertices = new List<Vector2>();
    private static List<Vector2> defVertices = new List<Vector2>();
    private static Vector2[] vs = new Vector2[2];
    private static List<Vector2> calculateUsingVertices = new List<Vector2>();

    /// <summary>
    /// 放大倍率
    /// </summary>
    private static float scaleRatio = 1.001f;

    private static void ReadyGetPoints(Collider2D atk, Collider2D def, float scale = 1.001f)
    {
        scale = scale != scaleRatio ? scale : scaleRatio;
        atkVertices.Clear();
        defVertices.Clear();

        GetColliderVertices(atk, atkVertices, scale);
        GetColliderVertices(def, defVertices, scale);
    }

    /// <summary>
    /// 找兩碰撞框邊長交疊點
    /// </summary>
    public static void GetColliderIntersectPoints(Collider2D atk, Collider2D def, List<Vector2> points, float scale = 1.001f)
    {
        ReadyGetPoints(atk, def, scale);
        GetColliderIntersectPoints(points);
    }

    private static void GetColliderIntersectPoints(List<Vector2> points)
    {
        //找交疊點
        for (int i = 0; i < atkVertices.Count; i++)
        {
            Vector2 a1 = atkVertices[i];
            Vector2 a2 = atkVertices[(i + 1) % atkVertices.Count];

            for (int j = 0; j < defVertices.Count; j++)
            {
                Vector2 b1 = defVertices[j];
                Vector2 b2 = defVertices[(j + 1) % defVertices.Count];

                if (TryGetIntersectPoint(a1, a2, b1, b2, out Vector2 p))
                {
                    points.Add(p);
                }
            }
        }
    }

    /// <summary>
    /// 找兩碰撞框邊內部碰撞點
    /// </summary>
    public static void GetColliderInnerIntersectPoints(Collider2D atk, Collider2D def, List<Vector2> points, float scale = 1.001f)
    {
        ReadyGetPoints(atk, def, scale);
        GetColliderInnerIntersectPoints(points);
    }

    private static void GetColliderInnerIntersectPoints(List<Vector2> points)
    {
        #region 受框在攻框中的內部碰撞點
        for (int i = 0; i < defVertices.Count; i++)
        {
            if (CheckPointInArea(atkVertices, defVertices[i], true))
            {
                points.Add(defVertices[i]);
            }
        }
        #endregion

        for (int i = 0; i < atkVertices.Count; i++)
        {
            #region 攻框在受框中的內部碰撞點
            if (CheckPointInArea(defVertices, atkVertices[i], true))
            {
                points.Add(atkVertices[i]);
            }
            #endregion
        }
    }

    public static void GetCollisionOverlapPoints(Collider2D atk, Collider2D def, List<Vector2> points, float scale = 1.001f)
    {
        ReadyGetPoints(atk, def, scale);
        GetColliderIntersectPoints(points);
        GetColliderInnerIntersectPoints(points);
    }

    public static void GetColliderVertices(Collider2D collider2D, List<Vector2> vertices, float extraScale = 1.001f)
    {
        extraScale = extraScale != scaleRatio ? extraScale : scaleRatio;
        if (collider2D is PolygonCollider2D polygon)
        {
            GetPolygonVerticesOrderByCCW(polygon, vertices, extraScale);
        }
        else if (collider2D is BoxCollider2D box)
        {
            Vector2 halfSize = box.size * 0.5f;
            //逆時針排列
            Vector2[] directions = {
            new Vector2(1, 1),   // 右上
                new Vector2(-1, 1),  // 左上
                new Vector2(-1, -1), // 左下
                new Vector2(1, -1)   // 右下
            };

            foreach (var dir in directions)
            {
                Vector2 localV = (halfSize * dir) + box.offset;
                localV *= extraScale; // 套用額外微調縮放
                vertices.Add(collider2D.transform.TransformPoint(localV));
            }
        }
    }

    private static void GetPolygonVerticesOrderByCCW(PolygonCollider2D polygon, List<Vector2> vertices, float extraScale)
    {
        GetPolygonPointCoordinate(polygon, vertices, extraScale);
        Vector2 center = polygon.bounds.center;
        vertices.OrderBy(p =>
        {
            return Mathf.Atan2(p.y - center.y, p.x - center.x);
        });
    }

    //因觸發TriggerEnter時，有機會兩碰撞框沒有任何重疊，研判是精度問題，故稍微放大一點點
    private static void GetPolygonPointCoordinate(PolygonCollider2D polygon, List<Vector2> vertices, float extraScale)
    {
        Vector2 center = polygon.bounds.center;
        foreach (var point in polygon.points)
        {
            Vector2 v = (point + polygon.offset) * extraScale;
            vertices.Add(polygon.transform.TransformPoint(v));
        }
    }

    public static double CaculateCross(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        Vector2 r = a2 - a1;
        Vector2 s = b2 - b1;

        //計算叉積，公式 x1 * y2 - y1 * x2
        //結果不為 0，可以確定兩向量不平行，有機會相交
        double rs = (double)(r.x * s.y) - (double)(r.y * s.x);
        return rs;
    }

    public static bool TryGetIntersectPoint(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 point)
    {
        point = Vector2.zero;

        //r
        double rx = (double)a2.x - a1.x;
        double ry = (double)a2.y - a1.y;

        //s
        double sx = (double)b2.x - b1.x;
        double sy = (double)b2.y - b1.y;

        double rs = CaculateCross(a1, a2, b1, b2);

        //叉積數值結果
        //順時針偏移 為負
        //逆時針偏移 為正
        //平行 為0
        if (Mathf.Abs((float)rs) < 1e-6f)
            return false;

        double dx = (double)b1.x - a1.x;
        double dy = (double)b1.y - a1.y;

        //將兩向量看成
        //A => P(t) = a1 + t * r
        //B => P(u) = b1 + u * s
        //t 跟 u 為常數
        //假如有交點，必定成立 => a1 + t * r =  b1 + u * s
        double t = (dx * sy - dy * sx) / rs; // ((B1 - A1) * s) / r * s
        double u = (dx * ry - dy * rx) / rs; // ((B1 - A1) * r) / r * s

        //交點如果在 (a2-a1) 和 (b2-b1) 的線段上，t 和 u 應該為大於等於0且小於等於1
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            //使用 t
            Vector2 res = new Vector2((float)(a1.x + t * rx), (float)(a1.y + t * ry)); //a1 + t * r

            //使用 u
            //Vector2 res = new Vector2((float)(b1.x + u * sx), (float)(b1.y + u * sy)); //b1 + u * s

            point = res; 
            return true;
        }
        return false;
    }

    private static bool CheckPointInArea(List<Vector2> vertices, Vector2 point, bool conatinLinePoint)
    {
        float preCross = 0;

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 v1 = vertices[i];
            Vector2 v2 = vertices[(i + 1) % vertices.Count];

            float cross = (v2.x - v1.x) * (point.y - v1.y) - (v2.y - v1.y) * (point.x - v1.x);

            if (i == 0)
            {
                preCross = cross;
            }
            else
            {
                if (conatinLinePoint)
                {
                    if (cross * preCross < 0)
                        return false;
                }
                else
                {
                    if (cross * preCross <= 0)
                        return false;
                }
            }
        }
        return true;
    }

    public static Vector2 GetMidPoint(float facing, List<Vector2> points)
    {
        if (points == null || points.Count <= 1)
            return Vector2.zero;
        else if (points.Count == 1)
            return points[0];

        if (facing > 0)
        {
            vs = new Vector2[2] { Vector2.negativeInfinity, Vector2.negativeInfinity };
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].x >= vs[0].x)
                {
                    vs[1] = vs[0];
                    vs[0] = points[i];
                    continue;
                }
                else if (points[i].x >= vs[1].x)
                {
                    vs[1] = points[i];
                    continue;
                }
            }
        }
        else
        {
            vs = new Vector2[2] { Vector2.positiveInfinity, Vector2.positiveInfinity };
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].x <= vs[0].x)
                {
                    vs[1] = vs[0];
                    vs[0] = points[i];
                    continue;
                }
                else if (points[i].x <= vs[1].x)
                {
                    vs[1] = points[i];
                    continue;
                }
            }
        }

        return (vs[0] + vs[1]) * 0.5f;
    }

    public static Vector2 GetColliderVertexWithX(Collider2D collider2D, bool isMinX, float scale = 1.001f)
    {
        scale = scale != scaleRatio ? scale : scaleRatio;

        calculateUsingVertices.Clear();
        GetColliderVertices(collider2D, calculateUsingVertices, scale);

        Vector2 res = new Vector2(isMinX ? float.PositiveInfinity : float.NegativeInfinity, float.PositiveInfinity);
        foreach (var vertex in calculateUsingVertices)
        {
            bool isTrue = (isMinX && vertex.x < res.x) || (!isMinX && vertex.x > res.x) || (vertex.x == res.x && vertex.y < res.y);
            if (isTrue)
                res = vertex;
        }
        return res;
    }

    public static void GetIntersectPoints(Vector2[] vertices, Vector2 v1, Vector2 v2, List<Vector2> points)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 p1 = vertices[i];
            Vector2 p2 = vertices[(i + 1) % vertices.Length];

            if (TryGetIntersectPoint(p1, p2, v1, v2, out Vector2 point))
            {
                points.Add(point);
            }
        }
    }

    /// <summary>
    /// 獲得過一點的垂直向量
    /// </summary>
    public static Vector2 GetPerpendicularVector(Vector2 crossPoint, Vector2 p1, Vector2 p2)
    {
        Vector2 v = p2 - p1;
        Vector2 u = crossPoint - p1;

        float t = Vector2.Dot(v, u) / v.sqrMagnitude;
        Vector2 d = p1 + t * v;

        Vector2 res = d - crossPoint;
        return res;
    }
}
