using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyHardAuthoring : MonoBehaviour
{
    private class Baker : Baker<EnemyHardAuthoring>
    {
        public override void Bake(EnemyHardAuthoring authoring)
        {
            AddComponent<EnemyHardPatternTag>(GetEntity(TransformUsageFlags.Dynamic));
        }
    }
}