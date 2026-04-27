using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct RaycastTestSystem : ISystem
{
    public void OnUpdate(ref SystemState state) 
    {
        if (!Input.GetKeyDown(KeyCode.R))
            return;

        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        var input = new RaycastInput()
        {
            Start = new float3(0, 10, 0),
            End = new float3(0, -1, 0),
            Filter = CollisionFilter.Default
        };

        if (physicsWorld.CastRay(input, out var hit))
        {
            Debug.Log($"打到 entity {hit.Entity.Index}，位置 {hit.Position}，法線 {hit.SurfaceNormal}");
        }
        else
        {
            Debug.Log("沒打到任何東西");
        }
    }
}