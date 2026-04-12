using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct RadialShooterConfig : IComponentData
{
    //砲台數值
    public float EmissionDirectionCount;
    public float FireRate;

    //砲彈實體
    public Entity Prefab;

    //砲彈數值
    public float Speed;
    public float3 Direction;
}

[BurstCompile]
public partial struct RadialShooterSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state) 
    {
        //建立生成Job
    }
}