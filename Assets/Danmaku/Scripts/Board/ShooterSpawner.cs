using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System;

public interface IShooterSpawner
{
    void Spawn(Vector3 worldPos, ShooterBaseData data);
}

public class RadialSpawner : IShooterSpawner
{
    public void Spawn(Vector3 worldPos, ShooterBaseData data)
    {
        RadialShooterData rd = data as RadialShooterData;

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = manager.CreateEntityQuery(typeof(SpawnRegistry));
        SpawnRegistry config = entityQuery.GetSingleton<SpawnRegistry>();

        var entity = manager.CreateEntity(typeof(RadialShooterConfig), typeof(LocalTransform));
        manager.SetComponentData(entity, new RadialShooterConfig
        {
            Prefab = config.RadialEntity,
            ShooterPosition = worldPos,
            EmissionDirectionCount = rd.DirCount,
            FireRate = rd.FireRate,
            Speed = rd.MoveSpeed,
            ElapsedTime = rd.FireRate
        });
    }
}

public class OrbitSpawner : IShooterSpawner
{
    public void Spawn(Vector3 worldPos, ShooterBaseData data)
    {
        OrbitShooterData ob = data as OrbitShooterData;

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = manager.CreateEntityQuery(typeof(SpawnRegistry));
        SpawnRegistry config = entityQuery.GetSingleton<SpawnRegistry>();

        var entity = manager.CreateEntity(typeof(OrbitShooterConfig), typeof(LocalTransform));
        manager.SetComponentData(entity, new OrbitShooterConfig
        {
            Prefab = config.OrbitEntity,
            ShooterPosition = worldPos,
            EmissionDirectionCount = ob.DirCount,
            ObjectCount = ob.ObjectCount,
            Speed = ob.RotateSpeed
        });
    }
}

public class ShooterFactory
{
    private Dictionary<System.Type, IShooterSpawner> shooterDic = new Dictionary<System.Type, IShooterSpawner>();

    public ShooterFactory()
    {
        shooterDic = new Dictionary<Type, IShooterSpawner>()
        {
            { typeof(RadialShooterData), new RadialSpawner()},
            { typeof(OrbitShooterData), new OrbitSpawner()}
        };
    }

    public void Spawn(Vector3 worldPos, ShooterBaseData data)
    {
        Type type = data.GetType();
        if (shooterDic.TryGetValue(type, out IShooterSpawner spawner))
        {
            spawner.Spawn(worldPos, data);
        }
    }
}

public class ShooterSpawner : MonoBehaviour
{
    public string ShooterDatabasePath;

    private ShooterDatabase shooterDatabase = null;
    private ShooterFactory shooterFactory = null;


    private void Start()
    {
        shooterDatabase = Resources.Load<ShooterDatabase>(ShooterDatabasePath); //先用Resources Load，之後改用其他載入方式
        shooterFactory = new ShooterFactory();
    }

    public void Spawn(Vector3 worldPos, int id)
    {
        if (shooterDatabase.TryGetShooterData(id, out ShooterBaseData data))
        {
            shooterFactory.Spawn(worldPos, data);
        }
    }
}
