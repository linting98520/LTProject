using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct RadialShooterConfig : IComponentData
{
    //砲台數值
    public float3 ShooterPosition;
    public int EmissionDirectionCount;
    public float FireRate;
    public float ElapsedTime;

    //砲彈實體
    public Entity Prefab;

    //砲彈數值
    public float Speed;

    public float BulletDamage;
    public float BulletLifetime;
}

[BurstCompile]
public partial struct RadialShooterSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RadialShooterConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) 
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        state.Dependency = new RadialShooterSpawnJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            Ecb = ecb
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct RadialShooterSpawnJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute([ChunkIndexInQuery] int sortKey, ref RadialShooterConfig config)
    {
        config.ElapsedTime += DeltaTime;
        if (config.ElapsedTime < config.FireRate) return;
        config.ElapsedTime = 0f;

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
            float3 dir = new float3(math.cos(currentAngle), 0, math.sin(currentAngle));
            float3 spawnPosition = config.ShooterPosition + (dir * 2);
            BulletSpawnHelper.SpawnLinearBullet(ref Ecb, sortKey, in bulletParams, spawnPosition, dir);
        }
    }
}