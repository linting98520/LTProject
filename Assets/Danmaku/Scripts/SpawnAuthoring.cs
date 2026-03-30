using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _spawnObj;
    [SerializeField] private float _spawnRate;

    public class SpawnBaker : Baker<SpawnAuthoring>
    {
        public override void Bake(SpawnAuthoring authoring)
        {
            if (authoring._spawnObj == null) return;

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnComponent
            {
                Prefab = GetEntity(authoring._spawnObj, TransformUsageFlags.Dynamic),
                SpawnRate = authoring._spawnRate
            });

            AddBuffer<SpawnedElement>(entity);
        }
    }
}