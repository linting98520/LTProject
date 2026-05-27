using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public partial struct DamageEvent : IComponentData
{
    public Entity Target; //受擊目標
    public float DamageValue; //受擊傷害
}

[BurstCompile]
[UpdateAfter(typeof(BulletMoveAndHitSystem))]
public partial struct DamageSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DamageEvent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                           .CreateCommandBuffer(state.WorldUnmanaged);

        var healthLookup = SystemAPI.GetComponentLookup<HealthData>();

        foreach (var (damageData, entity) in SystemAPI.Query<RefRO<DamageEvent>>().WithEntityAccess())
        {
            if (healthLookup.HasComponent(damageData.ValueRO.Target))
            {
                HealthData hp = healthLookup[damageData.ValueRO.Target];
                hp.Life -= damageData.ValueRO.DamageValue;
                healthLookup[damageData.ValueRO.Target] = hp;
            }
            ecb.DestroyEntity(entity); //刪除[傷害事件] 不是刪除受擊物件
        }
    }
}
