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
        // 使用 BeginSimulation 的 ECB
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        // 平行化發送請求
        new IssueRequestJob { Ecb = ecb }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct IssueRequestJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;

    // 只抓取帶有 SpawnMultiComponent 的實體
    private void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, in SpawnMultiComponent spawnMulti, in Unity.Transforms.LocalTransform transform)
    {
        for (int i = 0; i < spawnMulti.AmountPerWave; i++)
        {
            // 在平行執行緒中建立「工單實體」
            Entity req = Ecb.CreateEntity(sortKey);
            Ecb.AddComponent(sortKey, req, new SpawnRequest
            {
                PrefabToSpawn = spawnMulti.Prefab,
                Position = transform.Position
            });
        }

        // 重要：發完就移除組件，避免下一幀重複執行
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
        // 這裡改用 EndSimulation 的 ECB，拉開執行距離，減少 Sync Point 壓力
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
        // 1. 執行大量實例化
        Entity newEnemy = Ecb.Instantiate(sortKey, request.PrefabToSpawn);

        // 2. 設置 Transform (明確設定 Scale 確保看得見)
        var t = LocalTransform.FromPosition(request.Position);
        t.Scale = 1.0f;
        Ecb.SetComponent(sortKey, newEnemy, t);

        // 3. 銷毀工單
        Ecb.DestroyEntity(sortKey, reqEntity);
    }
}