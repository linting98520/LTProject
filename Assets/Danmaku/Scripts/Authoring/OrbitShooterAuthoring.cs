using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class OrbitShooterAuthoring : MonoBehaviour
{
    public class Baker : Baker<OrbitShooterAuthoring>
    {
        public override void Bake(OrbitShooterAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(e, new OrbitShooterConfig
            {
                ShooterPosition = float3.zero,
                EmissionDirectionCount = 1,
                ObjectCount = 1,
                Speed = 1,
                BulletLifetime = 1
            });
        }
    }
}
