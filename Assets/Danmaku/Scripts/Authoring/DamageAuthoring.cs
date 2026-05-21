using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Damage : IComponentData
{
    public float Value;
}

public class DamageAuthoring : MonoBehaviour
{
    public class Baker : Baker<DamageAuthoring>
    {
        public override void Bake(DamageAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(e, new Damage
            {
                Value = 0
            });
        }
    }
}
