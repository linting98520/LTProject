using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct PlayerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Vector2 dir = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
        {
            dir.x = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir.x = 1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            dir.y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dir.y = -1;
        }

        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity))
        {
            return;
        }

        if (SystemAPI.TryGetSingletonRW<PlayerInputComponent>(out var inputData))
        {
            inputData.ValueRW.MoveInput = dir;
        }

        LocalTransform playerPosTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);
        float3 playerPos = playerPosTransform.Position;

        new PlayerMoveJob()
        {
            Dt = SystemAPI.Time.DeltaTime,
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct PlayerMoveJob : IJobEntity
{
    public float Dt;

    public void Execute(ref LocalTransform transform, in PlayerComponent player, in PlayerInputComponent input)
    {
        float3 dir = new float3(input.MoveInput.x, input.MoveInput.y, 0);
        transform.Position += dir * Dt * player.Speed;
    }
}


