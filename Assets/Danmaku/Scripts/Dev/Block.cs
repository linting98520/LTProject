using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Entities.EntitiesJournaling;

public partial struct BlockData : IComponentData
{
    public Entity Prefab; //本體
    public float3 SpawnPos;
}

[BurstCompile]
public partial struct BlockSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BlockData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                           .CreateCommandBuffer(state.WorldUnmanaged)
                           .AsParallelWriter();

        state.Dependency = new BlockSpawnJob()
        {
            Ecb = ecb
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct BlockSpawnJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute([ChunkIndexInQuery]int sortKey, Entity entity, ref BlockData data)
    {
        Entity instance = Ecb.Instantiate(sortKey, data.Prefab);
        Ecb.SetComponent(sortKey, instance, LocalTransform.FromPositionRotationScale(data.SpawnPos, quaternion.identity, 5));

        //Block本體會掛載 HealthAuthoring 所以用Set不是Add
        Ecb.SetComponent(sortKey, instance, new HealthData
        {
            Life = 50
        });

        Ecb.DestroyEntity(sortKey, entity);
    }
}
