using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public struct PlayerComponent : IComponentData
{
    public float Speed;
}

public struct PlayerInputComponent : IComponentData
{
    public float2 MoveInput;
}