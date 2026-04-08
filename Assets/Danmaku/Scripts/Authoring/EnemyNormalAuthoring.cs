using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyNormalAuthoring : MonoBehaviour
{
    private class Baker : Baker<EnemyNormalAuthoring>
    {
        public override void Bake(EnemyNormalAuthoring authoring)
        {
            AddComponent<EnemyNormalPatternTag>(GetEntity(TransformUsageFlags.Dynamic));
        }
    }
}