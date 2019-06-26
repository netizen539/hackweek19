using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public struct ReadyToSpawnBulletComponent : IComponentData
{
	public Vector3 SpawnPosition;
	public Quaternion Direction;
	public Vector3 Forward;
}

public class SpawnProjectileSystem : ComponentSystem
{

	protected override void OnUpdate()
	{
		var getBulletTemplateQuery = GetEntityQuery(ComponentType.ReadOnly<BulletTemplateComponent>());
		var readyToSpawnQuery = GetEntityQuery(ComponentType.ReadOnly<ReadyToSpawnBulletComponent>());

		var spawnPoints = readyToSpawnQuery.ToEntityArray(Allocator.TempJob);

		foreach (var spawner in spawnPoints)
		{
			var bulletTemplates = getBulletTemplateQuery.ToEntityArray(Allocator.TempJob);
			var bulletTemplate = bulletTemplates[0];
			bulletTemplates.Dispose();
			var bullet = EntityManager.Instantiate(bulletTemplate);

			var bulletTemplateData = EntityManager.GetComponentData<BulletTemplateComponent>(bullet);
			EntityManager.RemoveComponent<BulletTemplateComponent>(bullet);
			var readyToSpawnComponent = EntityManager.GetComponentData<ReadyToSpawnBulletComponent>(spawner);
			var playerTranslation = EntityManager.GetComponentData<Translation>(spawner);
			var playerMovementComponent = EntityManager.GetComponentData<MovementComponent>(spawner);

			EntityManager.RemoveComponent<ReadyToSpawnBulletComponent>(spawner);
			var projectileData = new ProjectileComponent
			{
				Speed = bulletTemplateData.Speed,
				lifeTime = bulletTemplateData.LifeTime,
				forward = readyToSpawnComponent.Forward
			};

			var movementData = new MovementComponent
			{
				playerDirectionAxis = playerMovementComponent.playerDirectionAxis,
				speed = bulletTemplateData.Speed
			};

			EntityManager.AddComponentData(bullet, projectileData);
			EntityManager.AddComponentData(bullet, movementData);
			EntityManager.SetComponentData(bullet, playerTranslation);
		}

		spawnPoints.Dispose();

	}

}
