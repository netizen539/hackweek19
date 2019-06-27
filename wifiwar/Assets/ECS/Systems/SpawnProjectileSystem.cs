using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

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
			var playerRotationComponent = EntityManager.GetComponentData<Rotation>(spawner);
			EntityManager.RemoveComponent<ReadyToSpawnBulletComponent>(spawner);
			var projectileData = new ProjectileComponent
			{
				Speed = bulletTemplateData.Speed,
				LifeTime = bulletTemplateData.LifeTime,
				Player = spawner
			};

			Quaternion playerRotation = playerRotationComponent.Value;
			Matrix4x4 m = Matrix4x4.Rotate(playerRotation);
			Vector3 bulletDirection = m.MultiplyPoint3x4(Vector3.forward).normalized;
			Vector3 bulletTranslation = new Vector3(playerTranslation.Value.x, playerTranslation.Value.y + 0.4f, playerTranslation.Value.z);
			bulletTranslation += bulletDirection * 0.7f;

			var movementData = new MovementComponent
			{
				playerDirectionAxis = new Vector2(bulletDirection.x, bulletDirection.z),
				speed = bulletTemplateData.Speed
			};

			EntityManager.AddComponentData(bullet, projectileData);
			EntityManager.AddComponentData(bullet, movementData);
			EntityManager.SetComponentData(bullet, new Translation { Value = bulletTranslation });
		}

		spawnPoints.Dispose();

	}

}
