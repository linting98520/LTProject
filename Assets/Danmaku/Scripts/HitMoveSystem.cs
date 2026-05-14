using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public struct NextPosition : IComponentData
{
    public float3 Value;
}

[BurstCompile] //要在移動之後做
[UpdateInGroup(typeof(TransformSystemGroup))]
public partial struct HitMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<NextPosition>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                           .CreateCommandBuffer(state.WorldUnmanaged)
                           .AsParallelWriter();

        var bulletFilter = new CollisionFilter()
        {
            BelongsTo = 1u << 8,
            CollidesWith = (1u << 0) | (1u << 9),
            GroupIndex = 0
        };

        state.Dependency = new HitMoveJob()
        {
            PhysicsWorld = physicsWorld,
            Ecb = ecb,
            Filter = bulletFilter
        }.ScheduleParallel(state.Dependency);
    }
}

public partial struct HitMoveJob : IJobEntity
{
    [ReadOnly] public PhysicsWorldSingleton PhysicsWorld;
    public EntityCommandBuffer.ParallelWriter Ecb;
    public CollisionFilter Filter;

    public void Execute([ChunkIndexInQuery] int sortKey, Entity entity, ref LocalTransform transform, in NextPosition next)
    {
        var input = new RaycastInput
        {
            Start = transform.Position,
            End = next.Value,
            Filter = Filter
        };

        if (PhysicsWorld.CastRay(input, out var hit))
        {
            // 等待實作給予傷害
            Ecb.DestroyEntity(sortKey, entity);
            return;
        }

        transform.Position = next.Value;   // 沒命中才真的移動
    }
}
