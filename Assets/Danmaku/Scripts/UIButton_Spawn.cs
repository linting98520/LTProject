using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class UIButton_Spawn : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Button _clearButton;

    [SerializeField] private int _spawnAmount;
    [SerializeField] private float3 _firstObjPos = 0;
    [SerializeField] private SpawnPatternUtility.SpawnPatternType _patternType;

    private void Start()
    {
        _button.onClick.AddListener(OnSpawn);
        _clearButton?.onClick.AddListener(OnCleanup);
    }

    private void OnSpawn()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(SpawnRegistry));

        if (entityQuery.HasSingleton<SpawnRegistry>())
        {
            SpawnRegistry config = entityQuery.GetSingleton<SpawnRegistry>();
            Entity entity = GetEntityType(_patternType, config);

            Entity cmdEntity = entityManager.CreateEntity(typeof(EnemySpawnComponent));
            entityManager.SetComponentData(cmdEntity, new EnemySpawnComponent()
            {
                AmountPerWave = _spawnAmount,
                Prefab = entity,
                FirstObjPosition = _firstObjPos,
                PatternType = _patternType
            });
        }

        entityQuery.Dispose();
    }

    private Entity GetEntityType(SpawnPatternUtility.SpawnPatternType type, SpawnRegistry config)
    {
        return type switch
        {
            SpawnPatternUtility.SpawnPatternType.Easy => config.EasyEnemyEntity,
            SpawnPatternUtility.SpawnPatternType.Normal => config.NormalEnemyEntity,
            SpawnPatternUtility.SpawnPatternType.Hard => config.HardEnemyEntity,
            _ => config.EasyEnemyEntity
        };
    }

    private void OnCleanup()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<EnemyEasyPatternTag>());
        if (!query.IsEmptyIgnoreFilter)
            Debug.Log($"Spawn => {_patternType.ToString()} is Exist");
        else
            Debug.Log($"Spawn => {_patternType.ToString()} is NOT Exist");

        query.Dispose();
        //Type type = GetComponent(_patternType);
        //entityManager.CreateEntity(type);
        //Debug.Log($"DestroyTypeName = {type.Name}");
    }

    private Type GetComponent(SpawnPatternUtility.SpawnPatternType type)
    {
        return type switch
        {
            SpawnPatternUtility.SpawnPatternType.Easy => typeof(EnemyEasyDeleteCommand),
            SpawnPatternUtility.SpawnPatternType.Normal => typeof(EnemyNormalDeleteCommand),
            SpawnPatternUtility.SpawnPatternType.Hard => typeof(EnemyHardDeleteCommand),
            _ => typeof(EnemyEasyDeleteCommand)
        };
    }
}
