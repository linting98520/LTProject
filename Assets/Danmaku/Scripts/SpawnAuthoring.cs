using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpawnedElement : IBufferElementData
{
    public Entity Value;
}

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
            AddComponent(entity, authoring.GetPatternTag());

            AddComponent(entity, new EnemySpawnComponent
            {
                Prefab = GetEntity(authoring._spawnObj, TransformUsageFlags.Dynamic),
                AmountPerWave = authoring._amount,
                FirstObjPosition = authoring._firstObjPos
            });

            AddBuffer<SpawnedElement>(entity);
        }
    }

    public ComponentType GetPatternTag()
    {
        return _patternType switch
        {
            SpawnPatternUtility.SpawnPatternType.Enemy_Easy => ComponentType.ReadWrite<EnemyEasyPatternTag>(),
            SpawnPatternUtility.SpawnPatternType.Enemy_Normal => ComponentType.ReadWrite<EnemyNormalPatternTag>(),
            SpawnPatternUtility.SpawnPatternType.Enemy_Hard => ComponentType.ReadWrite<EnemyHardPatternTag>(),
            _ => ComponentType.ReadWrite<EnemyEasyPatternTag>()
        };
    }
}