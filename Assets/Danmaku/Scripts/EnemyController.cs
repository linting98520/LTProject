using System.Collections;
using System.Collections.Generic;
using Unity.Cecil.Awesome.Ordering;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2.5f;
    [SerializeField] private float _distanceThreshold = 30.0f;

    [SerializeField] private Transform _playerTransform;

    private void Awake()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Move()
    {
        Vector2 dir = (_playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90.0f;
        Quaternion lookRot = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = lookRot;

        float dis = Vector2.SqrMagnitude(_playerTransform.position - transform.position);
        if (dis <= _distanceThreshold)
        {
            transform.position += -(Vector3)dir * Time.deltaTime * _moveSpeed;
        }
    }
}
