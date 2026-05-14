using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BlockAuthoring : MonoBehaviour
{
    public class Baker : Baker<BlockAuthoring>
    {
        public override void Bake(BlockAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(e, new BlockData
            {
                SpawnPos = float3.zero
            });
        }
    }
}
