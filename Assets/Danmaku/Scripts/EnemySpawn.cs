using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject _enemyObj;
    [SerializeField] private int _spawnCount = 0;
    [SerializeField] private float _spawnRadius = 15.0f;
    [SerializeField] private Transform _playerTransform;

    [SerializeField] private float _enemyMoveSpeed = 3f;
    [SerializeField] private float _enemyThreshold = 5f;

    private bool _isDone = false;
    private List<GameObject> _enemyList = new List<GameObject>();
    private TransformAccessArray _enemyTransformAccessArray;

    private void Awake()
    {
        _enemyTransformAccessArray = new TransformAccessArray(_spawnCount);
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector2 spawnPos = UnityEngine.Random.insideUnitCircle * _spawnRadius;
            GameObject obj = Instantiate(_enemyObj, spawnPos, Quaternion.identity);
            _enemyList.Add(obj);
            _enemyTransformAccessArray.Add(obj.transform);
        }
        _isDone = true;
    }

    private void Update()
    {
        if (_isDone)
        {
            EnemyMoveJob job = new EnemyMoveJob()
            {
                DeltaTime = Time.deltaTime,
                MoveSpeed = _enemyMoveSpeed,
                DistanceThreshold = _enemyThreshold,
                PlayerPos = _playerTransform.position
            };
            JobHandle handle = job.Schedule(_enemyTransformAccessArray);
            handle.Complete();
        }
    }

    private void OnDestroy()
    {
        if (_enemyTransformAccessArray.isCreated)
        {
            _enemyTransformAccessArray.Dispose();
        }
    }
}

[BurstCompile]
public struct EnemyMoveJob : IJobParallelForTransform
{
    public float DeltaTime;
    public float MoveSpeed;
    public float DistanceThreshold;
    public float3 PlayerPos;

    public void Execute(int index, TransformAccess transform)
    {
        float3 dir = PlayerPos - (float3)transform.position;
        float disSq = math.lengthsq(dir);

        if (disSq < 0.001f) return;

        float angle = math.atan2(dir.y, dir.x);
        angle -= math.radians(90);
        transform.rotation = quaternion.RotateZ(angle);


        //normalize = vector / vector length
        //normalize = vector * (1 / (vector lenght)平方且開根號)
        //math.rsqrt 倒數平方根
        float3 moveDir = dir * math.rsqrt(disSq); 
        float3 newPos = (float3)transform.position + (-moveDir) * DeltaTime * MoveSpeed;
        float3 res = math.select(transform.position, newPos, disSq <= DistanceThreshold * DistanceThreshold);

        transform.position = res;
    }
}
