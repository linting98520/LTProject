using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct SpawnComponent : IComponentData
{
    public Entity Prefab;
    public float SpawnRate;
    public float NextSpawnTime;
}

public struct SpawnedElement : IBufferElementData
{
    public Entity Value;
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

public struct SpawnConfig : IComponentData
{
    public Entity PrefabEntity;
}

#region SpawnPatternUtility.SpawnPatternType
public struct EasyPatternTag : IComponentData { }
public struct NormalPatternTag : IComponentData { }
public struct HardPatternTag : IComponentData { }
#endregion