using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct OrbitShooterConfig : IComponentData
{
    //砲台數值
    public float3 ShooterPosition;
    public int EmissionDirectionCount;
    public int ObjectCount;

    //砲彈實體
    public Entity Prefab;

    public float Speed;
    public float BulletDamage;
    public float BulletLifetime;
}

[BurstCompile]
public partial struct OrbitShooterSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OrbitShooterConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) 
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        state.Dependency = new OrbitShooterSpawnJob
        {
            Ecb = ecb
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct OrbitShooterSpawnJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, ref OrbitShooterConfig config)
    {
        var bulletParams = new BulletSpawnParams
        {
            Prefab = config.Prefab,
            Speed = config.Speed,
            Damage = config.BulletDamage,
            Lifetime = config.BulletLifetime
        };

        float angleStep = (math.PI * 2f) / config.EmissionDirectionCount;
        for (int i = 0; i < config.EmissionDirectionCount; i++)
        {
            float currentAngle = i * angleStep;
            for (int x = 0; x < config.ObjectCount; x++)
            {
                float radius = (x + 1) * 2f;
                BulletSpawnHelper.SpawnOrbitBullet(ref Ecb, sortKey, in bulletParams, config.ShooterPosition, radius, currentAngle);
            }
        }

        Ecb.RemoveComponent<OrbitShooterConfig>(sortKey, entity);
    }
}