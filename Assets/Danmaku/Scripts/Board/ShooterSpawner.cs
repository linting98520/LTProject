using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public abstract class ShooterSpawnerBase
{
    public virtual void Spawn(Vector3 worldPos, ShooterBaseData data)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = manager.CreateEntityQuery(typeof(SpawnRegistry));
        SpawnRegistry config = entityQuery.GetSingleton<SpawnRegistry>();
        SetComponent(manager, config, worldPos, data);
    }

    public abstract void SetComponent(EntityManager manager, SpawnRegistry config, Vector3 worldPos, ShooterBaseData data);
}

public class RadialSpawner : ShooterSpawnerBase
{
    public override void SetComponent(EntityManager manager, SpawnRegistry config, Vector3 worldPos, ShooterBaseData data)
    {
        RadialShooterData rd = data as RadialShooterData;
        var entity = manager.CreateEntity(typeof(RadialShooterConfig), typeof(LocalTransform));
        manager.SetComponentData(entity, new RadialShooterConfig
        {
            Prefab = config.RadialBulletEntity,
            ShooterPosition = worldPos,
            EmissionDirectionCount = rd.DirCount,
            FireRate = rd.FireRate,
            Speed = rd.MoveSpeed,
            ElapsedTime = rd.FireRate,
            BulletLifetime = rd.BulletLifeTime
        });
    }
}

public class OrbitSpawner : ShooterSpawnerBase
{
    public override void SetComponent(EntityManager manager, SpawnRegistry config, Vector3 worldPos, ShooterBaseData data)
    {
        OrbitShooterData ob = data as OrbitShooterData;
        var entity = manager.CreateEntity(typeof(OrbitShooterConfig), typeof(LocalTransform));
        manager.SetComponentData(entity, new OrbitShooterConfig
        {
            Prefab = config.OrbitBulletEntity,
            ShooterPosition = worldPos,
            EmissionDirectionCount = ob.DirCount,
            ObjectCount = ob.ObjectCount,
            Speed = ob.RotateSpeed,
            BulletLifetime = ob.BulletLifeTime
        });
    }
}
public class BlockSpanwer : ShooterSpawnerBase
{
    public override void SetComponent(EntityManager manager, SpawnRegistry config, Vector3 worldPos, ShooterBaseData data)
    {
        var entity = manager.CreateEntity(typeof(BlockData), typeof(LocalTransform));
        manager.SetComponentData(entity, new BlockData
        {
            Prefab = config.BlockEntity,
            SpawnPos = worldPos
        });
    }
}

public class ShooterFactory
{
    private Dictionary<System.Type, ShooterSpawnerBase> shooterDic = new Dictionary<System.Type, ShooterSpawnerBase>();

    public ShooterFactory()
    {
        shooterDic = new Dictionary<Type, ShooterSpawnerBase>()
        {
            { typeof(RadialShooterData), new RadialSpawner()},
            { typeof(OrbitShooterData), new OrbitSpawner()},
            { typeof(ShooterBaseData), new BlockSpanwer()}
        };
    }

    public void Spawn(Vector3 worldPos, ShooterBaseData data)
    {
        Type type = data.GetType();
        if (shooterDic.TryGetValue(type, out ShooterSpawnerBase spawner))
        {
            spawner.Spawn(worldPos, data);
        }
    }
}

public class ShooterSpawner : MonoBehaviour
{
    public Vector3 SpawnPosOffest = Vector3.zero;

    private ShooterDatabase shooterDatabase = null;
    private ShooterFactory shooterFactory = null;


    private void Start()
    {
        shooterFactory = new ShooterFactory();
    }

    public void InjectShooterDB(ShooterDatabase database)
    {
        shooterDatabase  =database;
    }

    public bool TryToSpawn(Vector3 worldPos, int id)
    {
        if (shooterDatabase.TryGetShooterData(id, out ShooterBaseData data))
        {
            shooterFactory.Spawn(worldPos + SpawnPosOffest, data);
            return true;
        }
        return false;
    }
}
