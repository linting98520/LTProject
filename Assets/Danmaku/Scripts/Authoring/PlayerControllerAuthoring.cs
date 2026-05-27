using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerControllerAuthoring : MonoBehaviour
{
    public GameObject PlayerBulletObj;
    public float BulletSpeed;
    public float BulletDamage;
    public float BulletElapsedTime;
    public float BulletFireRate;

    //public class Baker : Baker<PlayerControllerAuthoring>
    //{
    //    public override void Bake(PlayerControllerAuthoring authoring)
    //    {
    //        var entity = GetEntity(TransformUsageFlags.Dynamic);
    //        AddComponent(entity, new FireRequest
    //        {
    //            ShooterPosition = float3.zero,
    //            Prefab = GetEntity(authoring.PlayerBulletObj, TransformUsageFlags.Dynamic),
    //            Speed = authoring.BulletSpeed,
    //            BulletDamage = authoring.BulletDamage,
    //            ElapsedTime = authoring.BulletElapsedTime,
    //            FireRate = authoring.BulletFireRate
    //        });
    //    }
    //}
}
