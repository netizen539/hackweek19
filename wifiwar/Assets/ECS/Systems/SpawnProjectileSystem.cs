﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
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
				lifeTime = bulletTemplateData.LifeTime,
				forward = readyToSpawnComponent.Forward
			};

			Quaternion playerRotation = playerRotationComponent.Value;
			Vector3 playerRotationEuler = playerRotation.eulerAngles;




			float2 playerDirectionAxis = playerMovementComponent.playerDirectionAxis;


			//var bulletTranslation = new float3(playerTranslation.Value.x += playerDirectionAxis.x * 5, playerTranslation.Value.y, playerTranslation.Value.z += playerDirectionAxis.y * 5);
			Vector3 bulletTranslation = new Vector3(playerTranslation.Value.x, playerTranslation.Value.y, playerTranslation.Value.z);
			//Quaternion newBulletRotaion = quaternion.AxisAngle(new float3(0F, 0F, 5F), math.atan2(playerTranslation.Value.x, playerTranslation.Value.z));
			Quaternion newBulletRotaion = Quaternion.AngleAxis(playerRotation.y, Vector3.up);
			bulletTranslation += playerRotationEuler.normalized * 5;

			Matrix4x4 m = Matrix4x4.Rotate(playerRotation);
			Vector3 bulletDirection = m.MultiplyPoint3x4(Vector3.forward).normalized;
			bulletTranslation += bulletDirection * 0.5f;
			//bulletTranslation += newBulletRotaion.normalized.eulerAngles * 5;

			//bulletTranslation.y = 0;
			//Debug.Log("bulletTranslation: " + bulletTranslation);
			//Debug.Log("playerTranslation: " + playerTranslation.Value);
			//Debug.Log("normalizedPlayerRotation: " + playerRotationEuler.normalized);

			var movementData = new MovementComponent
			{
				//playerDirectionAxis = playerMovementComponent.playerDirectionAxis,
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
