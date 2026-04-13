using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ShooterDevPanel : MonoBehaviour
{
    [BoxGroup("RadialShooter"), LabelText("發射器位置")]
    public float3 RadialPosition;

    [BoxGroup("RadialShooter"), LabelText("方位數量")]
    public int RadialDirCount;
    
    [BoxGroup("RadialShooter"), LabelText("間隔")]
    public float RadialFireRate;
    
    [BoxGroup("RadialShooter"), LabelText("速度")]
    public float RadialMoveSpeed;

    [BoxGroup("RadialShooter"), Button]
    public void RadialSpawn()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = manager.CreateEntityQuery(typeof(SpawnRegistry));
        SpawnRegistry config = entityQuery.GetSingleton<SpawnRegistry>();

        var entity = manager.CreateEntity(typeof(RadialShooterConfig), typeof(LocalTransform)); 
        manager.SetComponentData(entity, new RadialShooterConfig
        {
            Prefab = config.RadialEntity,
            ShooterPosition = RadialPosition,
            EmissionDirectionCount = RadialDirCount,
            FireRate = RadialFireRate,
            Speed = RadialMoveSpeed,
            ElapsedTime = RadialFireRate
        });
    }

    [BoxGroup("OrbitShooter"), LabelText("發射器位置")]
    public float3 OrbitPosition;

    [BoxGroup("OrbitShooter"), LabelText("方位數量")]
    public int OrbitDirCount;

    [BoxGroup("OrbitShooter"), LabelText("一排數量")]
    public int ObjectCount;

    [BoxGroup("OrbitShooter"), LabelText("速度")]
    public float OrbitMoveSpeed;

    [BoxGroup("OrbitShooter"), Button]
    public void OrbitSpawn()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = manager.CreateEntityQuery(typeof(SpawnRegistry));
        SpawnRegistry config = entityQuery.GetSingleton<SpawnRegistry>();

        var entity = manager.CreateEntity(typeof(OrbitShooterConfig), typeof(LocalTransform));
        manager.SetComponentData(entity, new OrbitShooterConfig
        {
            Prefab = config.OrbitEntity,
            ShooterPosition = OrbitPosition,
            EmissionDirectionCount = OrbitDirCount,
            ObjectCount = ObjectCount,
            Speed = OrbitMoveSpeed
        });
    }
}