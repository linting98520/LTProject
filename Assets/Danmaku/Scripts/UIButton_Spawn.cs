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
    }

    private void OnSpawn()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(SpawnConfig));

        if (entityQuery.HasSingleton<SpawnConfig>())
        {
            SpawnConfig config = entityQuery.GetSingleton<SpawnConfig>();
            Entity entity = config.PrefabEntity;

            Entity cmdEntity = entityManager.CreateEntity(typeof(EnemySpawnComponent));
            entityManager.SetComponentData(cmdEntity, new EnemySpawnComponent()
            {
                AmountPerWave = _spawnAmount,
                Prefab = entity,
                FirstObjPosition = _firstObjPos,
                PatternType = _patternType
            });
        }
    }
}
