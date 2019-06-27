using Unity.Entities;
public struct ProjectileComponent : IComponentData
{
	public int Speed;
	public float LifeTime;
	public Entity Player;
}
