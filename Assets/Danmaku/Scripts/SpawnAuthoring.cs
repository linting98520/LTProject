using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _spawnObj;
    [SerializeField] private float _spawnRate;
    [SerializeField] private int _amount;
    [SerializeField] private float3 _firstObjPos;
    [SerializeField] private SpawnPatternUtility.SpawnPatternType _patternType;

    public class SpawnBaker : Baker<SpawnAuthoring>
    {
        public override void Bake(SpawnAuthoring authoring)
        {
            if (authoring._spawnObj == null) return;

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemySpawnComponent
            {
                Prefab = GetEntity(authoring._spawnObj, TransformUsageFlags.Dynamic),
                AmountPerWave = authoring._amount,
                FirstObjPosition = authoring._firstObjPos,
                PatternType = authoring._patternType
            });

            AddBuffer<SpawnedElement>(entity);
        }
    }
}