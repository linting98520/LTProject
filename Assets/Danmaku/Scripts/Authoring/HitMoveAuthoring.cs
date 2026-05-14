using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HitMoveAuthoring : MonoBehaviour
{
    public class Baker : Baker<HitMoveAuthoring>
    {
        public override void Bake(HitMoveAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(e, new NextPosition
            {
                Value = 0
            });
        }
    }
}
