using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _spawnObj;
    [SerializeField] private float _spawnRate;
    [SerializeField] private int _amount;

    public class SpawnBaker : Baker<SpawnAuthoring>
    {
        public override void Bake(SpawnAuthoring authoring)
        {
            if (authoring._spawnObj == null) return;

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnMultiComponent
            {
                Prefab = GetEntity(authoring._spawnObj, TransformUsageFlags.Dynamic),
                AmountPerWave = authoring._amount
            });

            AddBuffer<SpawnedElement>(entity);
        }
    }
}