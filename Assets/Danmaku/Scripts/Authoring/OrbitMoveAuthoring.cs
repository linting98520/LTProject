using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class OrbitMoveAuthoring : MonoBehaviour
{
    public class Baker : Baker<OrbitMoveAuthoring>
    {
        public override void Bake(OrbitMoveAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(e, new OrbitMoveData
            {
                Center = float3.zero,
                Radius = 0,
                Speed = 0,
                Angle = 0
            });
        }
    }
}
