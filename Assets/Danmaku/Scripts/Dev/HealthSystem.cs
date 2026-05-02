using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct HealthData : IComponentData
{
    public int Life;
}

[BurstCompile]
[UpdateAfter(typeof(DamageSystem))]
public partial struct HealthSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HealthData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                           .CreateCommandBuffer(state.WorldUnmanaged)
                           .AsParallelWriter();
        state.Dependency = new HealthJob()
        {
            Ecb = ecb
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct HealthJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public void Execute([ChunkIndexInQuery]int sortKey, Entity entity, ref HealthData data)
    {
        if (data.Life <= 0)
        {
            Ecb.DestroyEntity(sortKey, entity);
        }
    }
}
