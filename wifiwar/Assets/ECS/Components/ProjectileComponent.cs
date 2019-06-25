using UnityEngine;
using Unity.Entities;
public struct ProjectileComponent : IComponentData
{
	public int Speed;
	public Vector3 forward;
}
