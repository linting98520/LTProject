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

    private void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, in EnemySpawnComponent spawnMulti)
    {
        for (int i = 0; i < spawnMulti.AmountPerWave; i++)
        {
            float3 pos = SpawnPatternUtility.GetPosition(spawnMulti.PatternType, i, spawnMulti.AmountPerWave, spawnMulti.FirstObjPosition);
            Entity req = Ecb.CreateEntity(sortKey);
            Ecb.AddComponent(sortKey, req, new SpawnRequest
            {
                PrefabToSpawn = spawnMulti.Prefab,
                Position = pos
            });

            switch (spawnMulti.PatternType)
            {
                case SpawnPatternUtility.SpawnPatternType.Easy:
                    Ecb.AddComponent<EasyPatternTag>(sortKey, req);
                    break;
                case SpawnPatternUtility.SpawnPatternType.Normal:
                    Ecb.AddComponent<NormalPatternTag>(sortKey, req);
                    break;
                case SpawnPatternUtility.SpawnPatternType.Hard:
                    Ecb.AddComponent<HardPatternTag>(sortKey, req);
                    break;
            }
        }

        Ecb.RemoveComponent<EnemySpawnComponent>(sortKey, entity);
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

//下次從這開始 改成泛型
[BurstCompile]
public partial struct DestroySpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, entity) in SystemAPI.Query<SpawnRequest>().WithAll<EasyPatternTag>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);

        ecb.Dispose();
    }
}