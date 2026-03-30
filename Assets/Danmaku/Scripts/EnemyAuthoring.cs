using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [field: SerializeField] public float _enemyMoveSpeed { get; private set; }
    [field: SerializeField] public float _enemyThreshold { get; private set; }

    public class Movebaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Entity playerEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new MoveComponent
            {
                Speed = authoring._enemyMoveSpeed,
                Threshold = authoring._enemyThreshold,
            });
        }
    }
}
