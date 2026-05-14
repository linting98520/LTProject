using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RadialShooterAuthoring : MonoBehaviour
{
    public class Baker : Baker<RadialShooterAuthoring>
    {
        public override void Bake(RadialShooterAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RadialShooterConfig
            {
                ShooterPosition = float3.zero,
                EmissionDirectionCount = 1,
                FireRate = 1,
                Speed = 1,
                ElapsedTime = 1,
                BulletLifetime = 1
            });
        }
    }
}
