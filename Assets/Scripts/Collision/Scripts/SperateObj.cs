using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SperateObj : MonoBehaviour
{
    private float movement = 10.0f;
    private float moveSpeed = 5.0f;

    private bool Moving = false;
    private Vector3 finalV3;

    public void Init(Vector3 p1, Vector3 p2, float movement, float speed)
    {
        this.movement = movement;
        moveSpeed = speed;
        PushSeparateObj(p1, p2);
    }

    public void StartMove()
    {
        Moving = true;
    }

    public void PushSeparateObj(Vector3 p1, Vector3 p2)
    {
        Vector2 prependicularVector = CollisionPointTool.GetPerpendicularVector(transform.position, p1, p2);
        Vector2 dirV2 = (-prependicularVector).normalized;
        Vector3 moveMovement = new Vector3(dirV2.x, dirV2.y) * movement;
        finalV3 = transform.position + moveMovement;
    }

    void Update()
    {
        if (Moving)
        {
            MoveUpdate();
        }
    }

    private void MoveUpdate()
    {
        if (transform.position != finalV3)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalV3, moveSpeed * Time.deltaTime);
        }
        else
        {
            Moving = false;
        }
    }
}
