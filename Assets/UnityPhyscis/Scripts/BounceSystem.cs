using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[BurstCompile]
public partial struct BouncySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state) 
    { 
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        foreach (var velocity in SystemAPI.Query<RefRW<PhysicsVelocity>>().WithAll<BounceComponent>())
        {
            velocity.ValueRW.Linear = new float3(0, 10f, 0);
        }
    }
}