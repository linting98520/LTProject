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
    public float3 Direction;

    public float BulletLifetime;
}

[BurstCompile]
public partial struct RadialShooterSpawnSystem : ISystem
{
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
        if (config.ElapsedTime >= config.FireRate)
        {
            float angleStep = (math.PI * 2f) / config.EmissionDirectionCount;

            for (int i = 0; i < config.EmissionDirectionCount; i++)
            {
                float currentAngle = i * angleStep;
                float3 dir = new float3(math.cos(currentAngle), 0, math.sin(currentAngle));
                quaternion rotation = quaternion.LookRotationSafe(dir, math.up());

                float3 spawnPosition = config.ShooterPosition + (dir * 2);

                Entity prefab = Ecb.Instantiate(sortKey, config.Prefab);
                Ecb.SetComponent(sortKey, prefab, LocalTransform.FromPositionRotation(spawnPosition, rotation));
                Ecb.AddComponent(sortKey, prefab, new LinearMoveData
                {
                    Speed = config.Speed,
                    Direction = dir
                });

                Ecb.AddComponent(sortKey, prefab, new ProjectileLifeTimeComponent
                {
                    RemainingTime = config.BulletLifetime
                });
            }
            config.ElapsedTime = 0;
        }
        config.ElapsedTime += DeltaTime;
    }
}