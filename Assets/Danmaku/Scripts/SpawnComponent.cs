using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SpawnComponent : IComponentData
{
    public Entity Prefab;
    public float SpawnRate;
    public float NextSpawnTime;
}

public struct SpawnedElement : IBufferElementData
{
    public Entity Value;
}

public partial struct SpawnMultiComponent : IComponentData
{
    public Entity Prefab;
    public int AmountPerWave;
    public bool Init;
}

public struct SpawnRequest : IComponentData
{
    public Entity PrefabToSpawn;
    public float3 Position;
}
