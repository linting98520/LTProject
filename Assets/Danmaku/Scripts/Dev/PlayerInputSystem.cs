using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct FireRequest : IComponentData
{
    public float3 ShooterPosition;

    public Entity Prefab;
    
    public float Speed;
    public float BulletDamage;
    public float BulletLifetime;

    public float ElapsedTime;
    public float FireRate;
}

[BurstCompile]
public partial struct FireRequestSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireRequest>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        state.Dependency = new FireJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            Ecb = ecb
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct FireJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute([ChunkIndexInQuery] int sortKey, ref FireRequest config, in LocalTransform transform)
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

        float3 dir = transform.Forward();
        float3 spawnPosition = transform.Position + dir * 2f;

        BulletSpawnHelper.SpawnLinearBullet(ref Ecb, sortKey, in bulletParams, spawnPosition, dir);
    }
}