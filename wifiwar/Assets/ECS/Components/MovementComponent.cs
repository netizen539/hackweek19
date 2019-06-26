using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public enum MovementDirection
{
    UP = 0,
    UP_RIGHT = 1,
    RIGHT = 2,
    DOWN_RIGHT = 3,
    DOWN = 4,
    DOWN_LEFT = 5,
    LEFT = 6,
    UP_LEFT = 7,
}

[Serializable]
public struct MovementComponent : IComponentData
{
    public MovementDirection direction;
    public float speed;
    public float2 playerDirectionAxis;
}
