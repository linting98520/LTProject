using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[DisableAutoCreation]
public partial struct SpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        new SpawnJob
        {
            ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
            Ecb = ecb.AsParallelWriter()
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct SpawnJob : IJobEntity
{
    public float ElapsedTime;
    public EntityCommandBuffer.ParallelWriter Ecb;

    private void Execute([ChunkIndexInQuery] int sortKey, ref SpawnComponent spawn, ref DynamicBuffer<SpawnedElement> buffer, in LocalTransform transform)
    {
        if (spawn.NextSpawnTime < ElapsedTime)
        {
            Entity entity = Ecb.Instantiate(sortKey, spawn.Prefab);
            LocalTransform newTransform = transform;

            int count = buffer.Length;
            newTransform.Position.x += count * 5;

            Ecb.SetComponent(sortKey, entity, newTransform);
            spawn.NextSpawnTime = ElapsedTime + spawn.SpawnRate;

            buffer.Add(new SpawnedElement { Value = entity });
        }
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct SpawnMultiSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        new IssueRequestJob { Ecb = ecb }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct IssueRequestJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;

    private void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, in SpawnMultiComponent spawnMulti)
    {
        int itemsPerRow = 10;
        int xOffset = 3;
        int yOffset = 5;
        float3 pos = spawnMulti.FirstObjPosition;

        for (int i = 0; i < spawnMulti.AmountPerWave; i++)
        {
            int row = i % itemsPerRow;
            int col = (int)math.floor(i / itemsPerRow);
            float3 newPos = pos + new float3(row * xOffset, col * yOffset, 0);

            Entity req = Ecb.CreateEntity(sortKey);
            Ecb.AddComponent(sortKey, req, new SpawnRequest
            {
                PrefabToSpawn = spawnMulti.Prefab,
                Position = newPos
            });
        }

        Ecb.RemoveComponent<SpawnMultiComponent>(sortKey, entity);
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SpawnMultiSystem))]
[BurstCompile]
public partial struct SpawnWorkerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        new SpawnWorkerJob { Ecb = ecb }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct SpawnWorkerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;

    private void Execute(Entity reqEntity, [ChunkIndexInQuery] int sortKey, in SpawnRequest request)
    {
        Entity newEnemy = Ecb.Instantiate(sortKey, request.PrefabToSpawn);

        var t = LocalTransform.FromPosition(request.Position);
        t.Scale = 1.0f;
        Ecb.SetComponent(sortKey, newEnemy, t);

        Ecb.DestroyEntity(sortKey, reqEntity);
    }
}