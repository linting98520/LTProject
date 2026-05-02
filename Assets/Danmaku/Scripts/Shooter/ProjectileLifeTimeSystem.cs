using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public struct ProjectileLifeTimeComponent : IComponentData
{
    public float RemainingTime;
}

[BurstCompile]
public partial struct ProjectileLifeTimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProjectileLifeTimeComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) 
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        
        state.Dependency = new ProjectileLifeTimeJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            Ecb = ecb
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct ProjectileLifeTimeJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute([ChunkIndexInQuery] int sortKey, Entity entity, ref ProjectileLifeTimeComponent component)
    {
        component.RemainingTime -= DeltaTime;
        if (component.RemainingTime <= 0)
        {
            Ecb.DestroyEntity(sortKey, entity);
        }
    }
}
