using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BouncyAuthoring : MonoBehaviour
{
    public class Baker : Baker<BouncyAuthoring>
    {
        public override void Bake(BouncyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BounceComponent>(entity);
        }
    }
}
