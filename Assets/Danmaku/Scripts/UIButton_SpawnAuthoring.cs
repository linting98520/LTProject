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
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpawnRegistry
            {
                EasyEnemyEntity = GetEntity(authoring.EasyEnemyObj, TransformUsageFlags.Dynamic),
                NormalEnemyEntity = GetEntity(authoring.NormalEnemyObj, TransformUsageFlags.Dynamic),
                HardEnemyEntity = GetEntity(authoring.HardEnemyObj, TransformUsageFlags.Dynamic),
            });
        }
    }
}
