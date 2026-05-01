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
            Start = new float3(0, 0.5f, 0),
            End = new float3(6, 0f, 0),
            Filter = new CollisionFilter
            {
                BelongsTo = 1u << 9, //player
                CollidesWith = 1u << 0, //default
                //CollidesWith = (1u << 8 | 1u << 0), //enemy or default
                GroupIndex = 0
            }
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