using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct FireRequest : IComponentData
{
    public Entity Prefab;
    
    public float Speed;
    public float BulletDamage;
    public float BulletLifetime;

    public float ElapsedTime;
    public float FireRate;
}

public partial struct ShooterState : IComponentData
{
    public float3 Position;
    public float3 Direction;
}

[BurstCompile]
public partial struct FireRequestSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireRequest>();
        state.RequireForUpdate<ShooterState>();
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

    public void Execute([ChunkIndexInQuery] int sortKey, ref FireRequest config, in ShooterState state)
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

        float3 dir = state.Direction;
        float3 spawnPosition = state.Position + dir * 2f;

        BulletSpawnHelper.SpawnLinearBullet(ref Ecb, sortKey, in bulletParams, spawnPosition, dir);
    }
}