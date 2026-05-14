using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

public struct LinearMoveData : IComponentData
{
    public float Speed;
    public float3 Direction;
}

[BurstCompile]
[UpdateInGroup(typeof(TransformSystemGroup))]
[UpdateBefore(typeof(HitMoveSystem))]
public partial struct LinearMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LinearMoveData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) 
    {
        state.Dependency = new LinearMoveJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct LinearMoveJob : IJobEntity
{
    public float DeltaTime;
    public void Execute(ref LocalTransform localTransform, in LinearMoveData data, ref NextPosition next)
    {
        //localTransform.Position += data.Direction * data.Speed * DeltaTime;
        next.Value = localTransform.Position + (data.Direction * data.Speed * DeltaTime);
    }
}