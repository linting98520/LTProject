using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct BulletSpawnParams
{
    public Entity Prefab;
    public float Speed;
    public float Damage;
    public float Lifetime;
}

public static class BulletSpawnHelper
{
    public static Entity SpawnLinearBullet(ref EntityCommandBuffer.ParallelWriter ecb, int sortKey, in BulletSpawnParams parameters, float3 position, float3 direction)
    {
        Entity bullet = ecb.Instantiate(sortKey, parameters.Prefab);

        quaternion rotation = quaternion.LookRotationSafe(direction, math.up());

        ecb.SetComponent(sortKey, bullet, LocalTransform.FromPositionRotation(position, rotation));

        ecb.SetComponent(sortKey, bullet, new LinearMoveData
        {
            Speed = parameters.Speed,
            Direction = direction
        });

        ecb.SetComponent(sortKey, bullet, new NextPosition { Value = position });

        ecb.SetComponent(sortKey, bullet, new Damage { Value = parameters.Damage });

        ecb.SetComponent(sortKey, bullet, new ProjectileLifeTimeData
        {
            RemainingTime = parameters.Lifetime
        });

        return bullet;
    }

    public static Entity SpawnOrbitBullet(ref EntityCommandBuffer.ParallelWriter ecb, int sortKey, in BulletSpawnParams parameters, float3 center, float radius, float startAngle)
    {
        Entity bullet = ecb.Instantiate(sortKey, parameters.Prefab);

        // 算初始位置（軌道上 startAngle 對應的點）
        float x = math.cos(startAngle) * radius;
        float z = math.sin(startAngle) * radius;
        float3 initialPos = center + new float3(x, 0, z);

        ecb.SetComponent(sortKey, bullet, LocalTransform.FromPosition(initialPos));

        ecb.SetComponent(sortKey, bullet, new OrbitMoveData
        {
            Center = center,
            Radius = radius,
            Speed = parameters.Speed,
            Angle = startAngle
        });

        ecb.SetComponent(sortKey, bullet, new NextPosition { Value = initialPos });

        ecb.SetComponent(sortKey, bullet, new Damage { Value = parameters.Damage });

        ecb.SetComponent(sortKey, bullet, new ProjectileLifeTimeData
        {
            RemainingTime = parameters.Lifetime
        });

        return bullet;
    }
}
