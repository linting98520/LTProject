using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public struct SpawnConfig : IComponentData
{
    public Entity PrefabEntity;
}

public class UIButton_Spawn : MonoBehaviour
{
    [SerializeField] private Button _button;

    [SerializeField] private int _spawnAmount = 1000;

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

            Entity cmdEntity = entityManager.CreateEntity(typeof(SpawnMultiComponent));
            entityManager.SetComponentData(cmdEntity, new SpawnMultiComponent()
            {
                AmountPerWave = _spawnAmount,
                Prefab = entity,
                FirstObjPosition = 0
            });
        }
    }
}
