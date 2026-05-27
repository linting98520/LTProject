using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ObjectileSpawnAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject RadialObj;
    [SerializeField] private GameObject RadialTowerObj;
    
    [SerializeField] private GameObject OrbitObj;
    [SerializeField] private GameObject OrbitTowerObj;
    
    [SerializeField] private GameObject BlockObj;
    [SerializeField] private GameObject PlayerBulletObj;

    private class UISpawnBtnBaker : Baker<ObjectileSpawnAuthoring>
    {
        public override void Bake(ObjectileSpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpawnRegistry
            {
                RadialBulletEntity = GetEntity(authoring.RadialObj, TransformUsageFlags.Dynamic),
                RadialShooterEntity = GetEntity(authoring.RadialTowerObj, TransformUsageFlags.Dynamic),

                OrbitBulletEntity = GetEntity(authoring.OrbitObj, TransformUsageFlags.Dynamic),
                OrbitShooterEntity = GetEntity(authoring.OrbitTowerObj, TransformUsageFlags.Dynamic),

                BlockEntity = GetEntity(authoring.BlockObj, TransformUsageFlags.Dynamic),
                PlayerBulletEntity = GetEntity(authoring.PlayerBulletObj, TransformUsageFlags.Dynamic),
            });
        }
    }
}
