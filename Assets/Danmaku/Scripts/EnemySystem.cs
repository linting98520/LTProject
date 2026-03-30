using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct MoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity))
        {
            return;
        }

        LocalTransform playerPosTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);
        float3 playerPos = playerPosTransform.Position;

        new MoveJob()
        {
            Dt = SystemAPI.Time.DeltaTime,
            PlayerPos = playerPos
        }.ScheduleParallel();
    }

}

[BurstCompile]
public partial struct MoveJob : IJobEntity
{
    public float Dt;
    public float3 PlayerPos;

    public void Execute(ref LocalTransform transform, in MoveComponent move)
    {
        float3 dir = PlayerPos - transform.Position;
        float disSq = math.lengthsq(dir);

        if (disSq < 0.001f) return;

        float angle = math.atan2(dir.y, dir.x);
        angle -= math.radians(90);
        transform.Rotation = quaternion.RotateZ(angle);

        float3 moveDir = dir * math.rsqrt(disSq);
        float3 newPos = transform.Position - moveDir * Dt * move.Speed;
        float threshold = move.Threshold;
        float3 res = math.select(transform.Position, newPos, disSq <= threshold * threshold);

        transform.Position = res;
    }
}