using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public Collider2D AtkCollider2D;
    public Collider2D DefCollider2D;
    public List<Vector2> CollisionPoints = new List<Vector2>();

    public List<Vector2> atkPoints = new List<Vector2>();
    public List<Vector2> defPoints = new List<Vector2>();

    #region Gizmos
    public float Gizmos_SphereSize = 1.0f;

    public Color Gizmos_CollisionPointColor = Color.white;
    public bool Gizmos_ShowCollisionPoint = true;

    public Color Gizmos_atkColliderColor = Color.white;
    public Color Gizmos_defColliderColor = Color.white;
    public bool Gizmos_ShowColliderPoint = true;
    #endregion

    private void ReadyGetPoints()
    {
        CollisionPoints.Clear();
        atkPoints.Clear();
        defPoints.Clear();
    }

    private void GetColliderPoints()
    {
        CollisionPointTool.GetColliderVertices(AtkCollider2D, atkPoints);
        CollisionPointTool.GetColliderVertices(DefCollider2D, defPoints);
    }

    public void StartCollisionIntersection()
    {
        ReadyGetPoints();
        CollisionPointTool.GetColliderIntersectPoints(AtkCollider2D, DefCollider2D, CollisionPoints);
        GetColliderPoints();
    }

    public void StartCollisionInnerIntersection()
    {
        ReadyGetPoints();
        CollisionPointTool.GetColliderInnerIntersectPoints(AtkCollider2D, DefCollider2D, CollisionPoints);
        GetColliderPoints();
    }

    public void StartCollisionOverlapPoints()
    {
        ReadyGetPoints();
        CollisionPointTool.GetCollisionOverlapPoints(AtkCollider2D, DefCollider2D, CollisionPoints);
        GetColliderPoints();
    }

    private void OnDrawGizmos()
    {
        if (Gizmos_ShowCollisionPoint)
        {
            Gizmos.color = Gizmos_CollisionPointColor;
            for (int i = 0; i < CollisionPoints.Count; i++)
            {
                Gizmos.DrawSphere(CollisionPoints[i], Gizmos_SphereSize);
            }
        }
        
        if (Gizmos_ShowColliderPoint)
        {
            Gizmos.color = Gizmos_atkColliderColor;
            for (int i = 0; i < atkPoints.Count; i++)
            {
                Gizmos.DrawSphere(atkPoints[i], Gizmos_SphereSize);
            }

            Gizmos.color = Gizmos_defColliderColor;
            for (int i = 0; i < defPoints.Count; i++)
            {
                Gizmos.DrawSphere(defPoints[i], Gizmos_SphereSize);
            }
        }
    }
}