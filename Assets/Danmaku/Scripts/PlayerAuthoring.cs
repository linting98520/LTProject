using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10;

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerComponent
            {
                Speed = authoring._moveSpeed
            });
            AddComponent<PlayerInputComponent>(entity);
        }
    }
}
