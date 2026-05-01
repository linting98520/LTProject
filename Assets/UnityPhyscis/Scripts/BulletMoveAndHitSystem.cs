using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Collections;

[BurstCompile]
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct BulletMoveAndHitSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BulletComponent>();
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
            BelongsTo = 1u << 9,
            CollidesWith = (1u << 0) | (1u << 8),
            GroupIndex = 0
        };

        state.Dependency = new BulletMoveAndHitJob()
        {
            Dt = SystemAPI.Time.DeltaTime,
            PhysicsWorld = physicsWorld,
            Ecb = ecb,
            BulletFilter = bulletFilter
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct BulletMoveAndHitJob : IJobEntity
{
    public float Dt;
    [ReadOnly] public PhysicsWorldSingleton PhysicsWorld;
    public EntityCommandBuffer.ParallelWriter Ecb;
    public CollisionFilter BulletFilter;

    public void Execute([ChunkIndexInQuery] int sortKey, Entity entity, ref LocalTransform transform, ref BulletComponent bulletData)
    {
        // 1. 算這幀的移動
        float3 startPos = transform.Position;
        float3 nextPos = startPos + bulletData.Velocity * Dt;

        // 2.從上一幀到這一幀打一條 raycast
        var raycast = new RaycastInput
        {
            Start = startPos,
            End = nextPos,
            Filter = BulletFilter
        };

        //命中 銷毀
        if (PhysicsWorld.CastRay(raycast, out Unity.Physics.RaycastHit hit))
        {
            Ecb.DestroyEntity(sortKey, entity);
            return;
        }

        //沒命中 繼續移動
        transform.Position = nextPos;

        // 3. 處理生命週期
        bulletData.RemainingLife -= Dt;
        if (bulletData.RemainingLife <= 0f)
        {
            Ecb.DestroyEntity(sortKey, entity);
        }
    }
}
