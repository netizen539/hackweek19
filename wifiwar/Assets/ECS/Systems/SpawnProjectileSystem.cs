using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Physics;

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
			Vector3 bulletTranslation = new Vector3(playerTranslation.Value.x, playerTranslation.Value.y, playerTranslation.Value.z);
			bulletTranslation += bulletDirection * 0.7f;

			var movementData = new MovementComponent
			{
				playerDirectionAxis = new Vector2(bulletDirection.x, bulletDirection.z),
				speed = bulletTemplateData.Speed
			};

			EntityManager.AddComponentData(bullet, projectileData);
			EntityManager.AddComponentData(bullet, movementData);
			EntityManager.SetComponentData(bullet, new Translation { Value = bulletTranslation });

			//Waiting for help with scaling..
			/*
			if (EntityManager.HasComponent<BigBulletPowerUpTag>(spawner) == true)
			{
				float4x4 compositeScale = EntityManager.GetComponentData<CompositeScale>(bullet).Value;
				//NonUniformScale scale = EntityManager.GetComponentData<NonUniformScale>(bullet);
				//NonUniformScale scale = new NonUniformScale { Value = new float3(3, 0.25f, 0.25f) };
				Scale scale = new Scale { Value = 2f };

				Debug.Log("Scale: " + scale);
				EntityManager.AddComponentData(bullet, scale);

				//PhysicsCollider physicsCollider = new PhysicsCollider();
				//EntityManager.SetComponentData(bullet, new CompositeScale { Value = new float4x4(compositeScale.c0 * 5, compositeScale.c1, compositeScale.c2 * 5, compositeScale.c3) });
			}
			*/

			if (EntityManager.HasComponent<ScattershotPowerUpTag>(spawner) == true)
			{
				// left bullet
				var leftBullet = EntityManager.Instantiate(bullet);
				var lefDirection = new Vector2(bulletDirection.x, bulletDirection.z);
				lefDirection = Quaternion.Euler(0, 0, -30) * lefDirection;
				var leftmovementData = new MovementComponent
				{
					playerDirectionAxis = lefDirection,
					speed = bulletTemplateData.Speed
				};

				EntityManager.SetComponentData(leftBullet, leftmovementData);

				//right bullet
				var rightBullet = EntityManager.Instantiate(bullet);
				var rightDirection = new Vector2(bulletDirection.x, bulletDirection.z);
				rightDirection = Quaternion.Euler(0, 0, +30) * rightDirection;
				var rightBulletData = new MovementComponent
				{
					playerDirectionAxis = rightDirection,
					speed = bulletTemplateData.Speed
				};

				EntityManager.SetComponentData(rightBullet, rightBulletData);

			}
		}

		spawnPoints.Dispose();

	}

}
