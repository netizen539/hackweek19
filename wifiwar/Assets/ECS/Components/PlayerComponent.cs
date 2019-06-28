using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PlayerComponent : IComponentData
{
	public uint kills;
	public float MaxCooldown;
	public float currentCoolDown;
}
