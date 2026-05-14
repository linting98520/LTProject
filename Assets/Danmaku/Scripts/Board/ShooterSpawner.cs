using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public abstract class ShooterSpawnerBase
{
    public virtual void Spawn(Vector3 worldPos, float scale, ShooterBaseData data)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = manager.CreateEntityQuery(typeof(SpawnRegistry));
        SpawnRegistry config = entityQuery.GetSingleton<SpawnRegistry>();
        SetComponent(manager, config, worldPos, scale, data);
    }

    public virtual void SetSpawnPos(EntityManager manager, Entity entity, Vector3 worldPos)
    {
        LocalTransform lt = manager.GetComponentData<LocalTransform>(entity);
        lt.Position = worldPos;
        manager.SetComponentData(entity, lt);
    }

    public abstract void SetComponent(EntityManager manager, SpawnRegistry config, Vector3 worldPos, float scale, ShooterBaseData data);
}

public class RadialSpawner : ShooterSpawnerBase
{
    public override void SetComponent(EntityManager manager, SpawnRegistry config, Vector3 worldPos, float scale, ShooterBaseData data)
    {
        RadialShooterData rd = data as RadialShooterData;
        Entity shooter = manager.Instantiate(config.RadialShooterEntity);
        SetSpawnPos(manager, shooter, worldPos);
        manager.SetComponentData(shooter, new RadialShooterConfig
        {
            Prefab = config.RadialBulletEntity,
            ShooterPosition = worldPos,
            EmissionDirectionCount = rd.DirCount,
            FireRate = rd.FireRate,
            Speed = rd.MoveSpeed,
            ElapsedTime = rd.FireRate,
            BulletLifetime = rd.BulletLifeTime
        });
        manager.SetComponentData(shooter, new HealthData
        {
            Life = data.Life
        });
    }
}

public class OrbitSpawner : ShooterSpawnerBase
{
    public override void SetComponent(EntityManager manager, SpawnRegistry config, Vector3 worldPos, float scale, ShooterBaseData data)
    {
        OrbitShooterData ob = data as OrbitShooterData;
        Entity shooter = manager.Instantiate(config.OrbitShooterEntity);
        SetSpawnPos(manager, shooter, worldPos);
        manager.SetComponentData(shooter, new OrbitShooterConfig
        {
            Prefab = config.OrbitBulletEntity,
            ShooterPosition = worldPos,
            EmissionDirectionCount = ob.DirCount,
            ObjectCount = ob.ObjectCount,
            Speed = ob.RotateSpeed,
            BulletLifetime = ob.BulletLifeTime
        });
        manager.SetComponentData(shooter, new HealthData
        {
            Life = data.Life
        });
    }
}
public class BlockSpanwer : ShooterSpawnerBase
{
    public override void SetComponent(EntityManager manager, SpawnRegistry config, Vector3 worldPos, float scale, ShooterBaseData data)
    {
        Entity block = manager.Instantiate(config.BlockEntity);
        SetSpawnPos(manager, block, worldPos);
        manager.SetComponentData(block, new BlockData
        {
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

    public void Spawn(Vector3 worldPos, float scale, ShooterBaseData data)
    {
        Type type = data.GetType();
        if (shooterDic.TryGetValue(type, out ShooterSpawnerBase spawner))
        {
            spawner.Spawn(worldPos, scale, data);
        }
    }
}

public class ShooterSpawner : MonoBehaviour
{
    public Vector3 SpawnPosOffest = Vector3.zero;
    public float SpawnScale = 1;

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
            shooterFactory.Spawn(worldPos + SpawnPosOffest, SpawnScale, data);
            return true;
        }
        return false;
    }
}
