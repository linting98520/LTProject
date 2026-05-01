using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BulletPrefabReference : IComponentData
{
    public Entity Value;
}

public class BulletPrefabAuthoring : MonoBehaviour
{
    public GameObject BulletPrefab;

    public class Baker : Baker<BulletPrefabAuthoring>
    {
        public override void Bake(BulletPrefabAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BulletPrefabReference()
            {
                Value = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}
