using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct ShooterConfig : IComponentData
{
    public Entity Prefab;
    public float FireRate;
}

public struct LinearBaseData : IComponentData
{
    public float Speed;
    public float3 Direction;
}


[BurstCompile]
public partial struct LinearMoveSystem : ISystem
{
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
    public void Execute(ref LocalTransform localTransform, in LinearBaseData data)
    {
        localTransform.Position += data.Direction * data.Speed * DeltaTime;
    }
}