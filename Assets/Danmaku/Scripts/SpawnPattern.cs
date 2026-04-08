using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public static class SpawnPatternUtility
{
    public enum SpawnPatternType
    {
        Enemy_Easy,
        Enemy_Normal,
        Enemy_Hard
    }

    public static float3 GetPosition(SpawnPatternType type, int index, int total, float3 center)
    {
        float3 res = 0;

        switch (type)
        {
            case SpawnPatternType.Enemy_Easy:
                int itemsPerRow = 10;
                res = center + new float3((index % itemsPerRow) * 2f, (index / itemsPerRow) * 2f, 0);
                break;
            case SpawnPatternType.Enemy_Normal:
                float angle = (math.PI * 2f / total) * index;
                res = center + new float3(math.cos(angle), math.sin(angle), 0) * 5f; // ¥b®| 5
                break;
            case SpawnPatternType.Enemy_Hard:
                float fanAngle = math.radians(60f); // 60«×®°§Î
                float startAngle = -fanAngle / 2f;
                float currentAngle = startAngle + (fanAngle / (total - 1)) * index;
                res = center + new float3(math.cos(currentAngle), math.sin(currentAngle), 0) * 8f;
                break;
            default:
                res = center;
                break;
        }

        return res;
    }
}
