using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HealthSystemAuthoring : MonoBehaviour
{
    public int MaxHealth = 100;
    public class Baker : Baker<HealthSystemAuthoring>
    {
        public override void Bake(HealthSystemAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HealthData()
            {
                Life = authoring.MaxHealth
            });
        }
    }
}
