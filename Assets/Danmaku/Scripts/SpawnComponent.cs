using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct SpawnRegistry : IComponentData
{
    public Entity EasyEnemyEntity;
    public Entity NormalEnemyEntity;
    public Entity HardEnemyEntity;
    public Entity RadialEntity;
    public Entity OrbitEntity;
}

public struct EnemySpawnComponent : IComponentData
{
    public Entity Prefab;
    public int AmountPerWave;
    public float3 FirstObjPosition;
    public SpawnPatternUtility.SpawnPatternType PatternType;
}

public struct SpawnRequest : IComponentData
{
    public Entity PrefabToSpawn;
    public float3 Position;
}


#region SpawnPatternUtility.SpawnPatternType
public struct EnemyEasyPatternTag : IComponentData { }
public struct EnemyEasyDeleteCommand : IComponentData { }

public struct EnemyNormalPatternTag : IComponentData { }
public struct EnemyNormalDeleteCommand : IComponentData { }

public struct EnemyHardPatternTag : IComponentData { }
public struct EnemyHardDeleteCommand : IComponentData { }
#endregion