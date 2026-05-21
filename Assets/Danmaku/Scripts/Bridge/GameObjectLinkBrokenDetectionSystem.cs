using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public struct EntityLinkBrokenEvent : IComponentData
{
    public int LinkedInstanceID;
    public LinkType Type;
}

[BurstCompile]
public partial struct GameObjectLinkBrokenDetectionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameObjectLink>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                           .CreateCommandBuffer(state.WorldUnmanaged)
                           .AsParallelWriter();
        state.Dependency = new DetectionJob
        {
            Ecb = ecb
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct DetectionJob: IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute([ChunkIndexInQuery] int sortKey, in HealthData hp, in GameObjectLink link)
    {
        if (hp.Life > 0f) return;

        var evt = Ecb.CreateEntity(sortKey);
        Ecb.AddComponent(sortKey, evt, new EntityLinkBrokenEvent
        {
            LinkedInstanceID = link.LinkedInstanceID,
            Type = link.Type
        });
    }
}