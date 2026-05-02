using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct LinearMoveData : IComponentData
{
    public float Speed;
    public float3 Direction;
}

[BurstCompile]
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
    public void Execute(ref LocalTransform localTransform, in LinearMoveData data)
    {
        localTransform.Position += data.Direction * data.Speed * DeltaTime;
    }
}