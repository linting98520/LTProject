using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct MoveComponent : IComponentData
{
    public float Speed;
    public float Threshold;
}
