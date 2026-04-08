using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UIButton_SpawnAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject EasyEnemyObj;
    [SerializeField] private GameObject NormalEnemyObj;
    [SerializeField] private GameObject HardEnemyObj;

    private class UISpawnBtnBaker : Baker<UIButton_SpawnAuthoring>
    {
        public override void Bake(UIButton_SpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            Entity easyPrefab = GetEntity(authoring.EasyEnemyObj, TransformUsageFlags.Dynamic);
            Entity normalPrefab = GetEntity(authoring.NormalEnemyObj, TransformUsageFlags.Dynamic);
            Entity hardPrefab = GetEntity(authoring.HardEnemyObj, TransformUsageFlags.Dynamic);

            AddComponent(entity, new SpawnRegistry()
            {
                EasyEnemyEntity = easyPrefab,
                NormalEnemyEntity = normalPrefab,
                HardEnemyEntity = hardPrefab,
            });
        }
    }
}
