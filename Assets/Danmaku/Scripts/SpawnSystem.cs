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

[BurstCompile]
public partial struct SpawnMultiSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (spawnMulti, transform, entity) in SystemAPI.Query<RefRW<SpawnMultiComponent>, LocalTransform>().WithEntityAccess())
        {
            for (int i = 0; i < spawnMulti.ValueRO.AmountPerWave; i++)
            {
                Entity req = ecb.CreateEntity();
                ecb.AddComponent(req, new SpawnRequest
                {
                    PrefabToSpawn = spawnMulti.ValueRO.Prefab,
                    Position = transform.Position // 注意：這裡直接用 .Position
                });
            }

            ecb.RemoveComponent<SpawnMultiComponent>(entity);
        }
    }
}

[BurstCompile]
public partial struct SpawnWorkerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 取得平行寫入的 ECB
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        // 啟動平行 Job
        new SpawnWorkerJob { Ecb = ecb }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct SpawnWorkerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;

    private void Execute(Entity reqEntity, in SpawnRequest request, [ChunkIndexInQuery] int sortKey)
    {
        // 1. 真正執行沉重的 Instantiate (由多核分擔)
        Entity newEnemy = Ecb.Instantiate(sortKey, request.PrefabToSpawn);

        // 2. 設定位置
        Ecb.SetComponent(sortKey, newEnemy, LocalTransform.FromPosition(request.Position));

        // 3. 重要：任務完成，銷毀這張「工單」
        Ecb.DestroyEntity(sortKey, reqEntity);
    }
}