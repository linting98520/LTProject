using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ProjectileLifeAuthoring : MonoBehaviour
{
    public class Baker : Baker<ProjectileLifeAuthoring>
    {
        public override void Bake(ProjectileLifeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ProjectileLifeTimeData
            {
                RemainingTime = 3
            });
        }
    }
}
