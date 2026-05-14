using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class LinearMoveAuthoring : MonoBehaviour
{
    public class Baker : Baker<LinearMoveAuthoring>
    {
        public override void Bake(LinearMoveAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new LinearMoveData
            {
                Speed = 0,
                Direction = 0
            });
        }
    }
}
