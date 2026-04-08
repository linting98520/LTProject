using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class ShowSomethings : MonoBehaviour
{
    [Button]
    public void ShowSpawnEntity(SpawnPatternUtility.SpawnPatternType _patternType)
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        //em.CreateEntityQuery(GetComponent(_patternType));
        EntityQuery query = em.CreateEntityQuery(ComponentType.ReadOnly<EnemyEasyPatternTag>());

        if (!query.IsEmptyIgnoreFilter)
            Debug.Log($"Spawn => {_patternType.ToString()} is Exist");
        else
            Debug.Log($"Spawn => {_patternType.ToString()} is NOT Exist");

        query.Dispose();
    }

    private Type GetComponent(SpawnPatternUtility.SpawnPatternType type)
    {
        return type switch
        {
            SpawnPatternUtility.SpawnPatternType.Easy => typeof(EnemyEasyDeleteCommand),
            SpawnPatternUtility.SpawnPatternType.Normal => typeof(EnemyNormalDeleteCommand),
            SpawnPatternUtility.SpawnPatternType.Hard => typeof(EnemyHardDeleteCommand),
            _ => typeof(EnemyEasyDeleteCommand)
        };
    }

    [Button]
    public void ShowCommandEntity(SpawnPatternUtility.SpawnPatternType _patternType)
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery query = em.CreateEntityQuery(GetCommand(_patternType));

        if (!query.IsEmptyIgnoreFilter)
            Debug.Log($"Cmd => {_patternType.ToString()} is Exist");
        else
            Debug.Log($"Cmd => {_patternType.ToString()} is NOT Exist");

        query.Dispose();
    }

    private Type GetCommand(SpawnPatternUtility.SpawnPatternType type)
    {
        return type switch
        {
            SpawnPatternUtility.SpawnPatternType.Easy => typeof(EnemyEasyDeleteCommand),
            SpawnPatternUtility.SpawnPatternType.Normal => typeof(EnemyNormalDeleteCommand),
            SpawnPatternUtility.SpawnPatternType.Hard => typeof(EnemyHardDeleteCommand),
            _ => typeof(EnemyEasyDeleteCommand)
        };
    }
}
