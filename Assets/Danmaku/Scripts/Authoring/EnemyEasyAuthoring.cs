using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyEasyAuthoring : MonoBehaviour
{
    private class Baker : Baker<EnemyEasyAuthoring>
    {
        public override void Bake(EnemyEasyAuthoring authoring)
        {
            AddComponent<EnemyEasyPatternTag>(GetEntity(TransformUsageFlags.Dynamic));
        }
    }
}