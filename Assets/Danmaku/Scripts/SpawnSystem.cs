using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct SpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        new SpawnRequestJob { Ecb = ecb }.ScheduleParallel();
    }
}

/// <summary>
/// 生成Enemy
/// </summary>
[BurstCompile]
public partial struct SpawnRequestJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;

    private void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, in EnemySpawnComponent compoment)
    {
        for (int i = 0; i < compoment.AmountPerWave; i++)
        {
            float3 pos = SpawnPatternUtility.GetPosition(compoment.PatternType, i, compoment.AmountPerWave, compoment.FirstObjPosition);
            Entity objEntity = Ecb.CreateEntity(sortKey);
            Ecb.AddComponent(sortKey, objEntity, new SpawnRequest
            {
                PrefabToSpawn = compoment.Prefab,
                Position = pos,
            });
        }

        Ecb.RemoveComponent<EnemySpawnComponent>(sortKey, entity);
    }
}

#region 通用生成

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SpawnSystem))]
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

    private void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, in SpawnRequest request)
    {
        Entity objEntity = Ecb.Instantiate(sortKey, request.PrefabToSpawn);

        var t = LocalTransform.FromPosition(request.Position);
        t.Scale = 1.0f;

        Ecb.SetComponent(sortKey, objEntity, t);

        Ecb.DestroyEntity(sortKey, entity);
    }
}

#endregion

#region 刪除Enemy

public abstract partial class DestroySystem<TTag, TCmd> : SystemBase 
    where TTag : unmanaged, IComponentData
    where TCmd : unmanaged, IComponentData
{
    private EntityQuery _targetQuery;
    private EntityQuery _commandQuery;

    protected override void OnCreate()
    {
        _targetQuery = GetEntityQuery(ComponentType.ReadOnly<TTag>());
        _commandQuery = GetEntityQuery(ComponentType.ReadOnly<TCmd>());
        RequireForUpdate(_commandQuery);
    }

    protected override void OnUpdate()
    {
        Debug.Log($"DestroySystem => {typeof(TTag).Name}");
        EntityManager.DestroyEntity(_targetQuery);
        EntityManager.DestroyEntity(_commandQuery);
    }
}

public partial class EasyDestroySystem : DestroySystem<EnemyEasyPatternTag, EnemyEasyDeleteCommand> { }
public partial class NormalDestroySystem : DestroySystem<EnemyNormalPatternTag, EnemyNormalDeleteCommand> { }
public partial class HardDestroySystem : DestroySystem<EnemyHardPatternTag, EnemyHardDeleteCommand> { }

#endregion