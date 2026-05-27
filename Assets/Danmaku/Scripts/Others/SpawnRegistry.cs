using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct SpawnRegistry : IComponentData
{
    public Entity RadialBulletEntity;
    public Entity RadialShooterEntity;
    
    public Entity OrbitBulletEntity;
    public Entity OrbitShooterEntity;
    
    public Entity BlockEntity;

    public Entity PlayerBulletEntity;
}