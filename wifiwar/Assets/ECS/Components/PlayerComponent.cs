﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PlayerComponent : IComponentData
{
    public int kills;
    public bool isPlayer1;
}
