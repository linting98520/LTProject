using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct OrbitMoveData : IComponentData
{
    public float3 Center;
    public float Radius;
    public float Speed;
    public float Angle;
}

[BurstCompile]
public partial struct OrbitMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new OrbitMoveJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct OrbitMoveJob : IJobEntity
{
    public float DeltaTime;

    private void Execute(ref LocalTransform transform, ref OrbitMoveData orbit)
    {
        orbit.Angle += orbit.Speed * DeltaTime;

        float x = math.cos(orbit.Angle) * orbit.Radius;
        float z = math.sin(orbit.Angle) * orbit.Radius;

        transform.Position = orbit.Center + new float3(x, 0, z);
    }
}
