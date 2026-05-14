using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ObjectileSpawnAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject EasyEnemyObj;
    [SerializeField] private GameObject NormalEnemyObj;
    [SerializeField] private GameObject HardEnemyObj;

    [SerializeField] private GameObject RadialObj;
    [SerializeField] private GameObject RadialTowerObj;
    
    [SerializeField] private GameObject OrbitObj;
    [SerializeField] private GameObject OrbitTowerObj;
    
    [SerializeField] private GameObject BlockObj;

    private class UISpawnBtnBaker : Baker<ObjectileSpawnAuthoring>
    {
        public override void Bake(ObjectileSpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpawnRegistry
            {
                EasyEnemyEntity = GetEntity(authoring.EasyEnemyObj, TransformUsageFlags.Dynamic),
                NormalEnemyEntity = GetEntity(authoring.NormalEnemyObj, TransformUsageFlags.Dynamic),
                HardEnemyEntity = GetEntity(authoring.HardEnemyObj, TransformUsageFlags.Dynamic),

                RadialBulletEntity = GetEntity(authoring.RadialObj, TransformUsageFlags.Dynamic),
                RadialShooterEntity = GetEntity(authoring.RadialTowerObj, TransformUsageFlags.Dynamic),

                OrbitBulletEntity = GetEntity(authoring.OrbitObj, TransformUsageFlags.Dynamic),
                OrbitShooterEntity = GetEntity(authoring.OrbitTowerObj, TransformUsageFlags.Dynamic),

                BlockEntity = GetEntity(authoring.BlockObj, TransformUsageFlags.Dynamic)
            });
        }
    }
}
