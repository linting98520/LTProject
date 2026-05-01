using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct BulletSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!Input.GetKeyDown(KeyCode.F))
            return;

        var prefabRef = SystemAPI.GetSingleton<BulletPrefabReference>();
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        float3 spawnPos = new float3(0, 0.5f, 0);
        float3 dir = new float3(1, 0, 0);
        float speed = 20.0f;

        Entity bullet = ecb.Instantiate(prefabRef.Value);

        //bullet 已有 LocalTransform 所以用SetComponent 如果沒有用AddComponent
        ecb.SetComponent(bullet, LocalTransform.FromPositionRotationScale(spawnPos, quaternion.identity, 0.2f));

        ecb.AddComponent(bullet, new BulletComponent()
        {
            Velocity = dir * speed,
            Damage = 10f,
            RemainingLife = 3f
        });

        //立刻執行
        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        Debug.Log($"發射子彈");
    }
}
