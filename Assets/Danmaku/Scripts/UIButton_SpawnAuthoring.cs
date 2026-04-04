using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UIButton_SpawnAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject SpawnObj;

    private class UISpawnBtnBaker : Baker<UIButton_SpawnAuthoring>
    {
        public override void Bake(UIButton_SpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            Entity convertPrefab = GetEntity(authoring.SpawnObj, TransformUsageFlags.Dynamic);

            AddComponent(entity, new SpawnConfig()
            {
                PrefabEntity = convertPrefab
            });
        }
    }
}
