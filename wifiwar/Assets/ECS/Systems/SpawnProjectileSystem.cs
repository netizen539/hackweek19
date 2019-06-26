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
	BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
	//EntityQuery bulletTemplateGroup;
	//EntityQuery readyToSpawnComponents;


	protected override void OnCreate()
	{
		m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
		//bulletTemplateGroup = GetEntityQuery(ComponentType.ReadOnly<BulletTemplateComponent>());
	}
	/*
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var spawnPoints = GetEntityQuery(ComponentType.ReadOnly<ReadyToSpawnBulletComponent>());

		foreach (var spawner in spawnPoints.ToComponentDataArray<ReadyToSpawnBulletComponent>(Allocator.TempJob))
		{
			var bulletTemplate = bulletTemplateGroup.ToEntityArray(Allocator.TempJob)[0];
			var bullet = EntityManager.Instantiate(bulletTemplate);
			var templateData = EntityManager.GetComponentData<BulletTemplateComponent>(bullet);
			EntityManager.RemoveComponent<BulletTemplateComponent>(bullet);
			var projectileData = new ProjectileComponent
			{
				Speed = templateData.Speed,
				lifeTime = templateData.LifeTime,
				forward = spawner.Forward
			};

			EntityManager.AddComponentData(bullet, projectileData);
		}


		

		// Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
		var job = new SpawnProjectileJob
		{
			CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
			bulletTemplateArray = bulletTemplateGroup.ToComponentDataArray<BulletTemplateComponent>(Allocator.TempJob)

		}.Schedule(this, inputDeps);

		// SpawnJob runs in parallel with no sync point until the barrier system executes.
		// When the barrier system executes we want to complete the SpawnJob and then play back the commands
		// (Creating the entities and placing them). We need to tell the barrier system which job it needs to
		// complete before it can play back the commands.
		m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

		return job;
		
	}
*/

	protected override void OnUpdate()
	{
		//using (var readyToSpawnQuery = GetEntityQuery(ComponentType.ReadOnly<ReadyToSpawnBulletComponent>()))

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
				var templateData = EntityManager.GetComponentData<BulletTemplateComponent>(bullet);
				EntityManager.RemoveComponent<BulletTemplateComponent>(bullet);
				var readyToSpawnComponent = EntityManager.GetComponentData<ReadyToSpawnBulletComponent>(spawner);
				EntityManager.RemoveComponent<ReadyToSpawnBulletComponent>(spawner);
				var projectileData = new ProjectileComponent
				{
					Speed = templateData.Speed,
					lifeTime = templateData.LifeTime,
					forward = readyToSpawnComponent.Forward
				};

				EntityManager.AddComponentData(bullet, projectileData);
			}

			spawnPoints.Dispose();

			//getBulletTemplateQuery.Dispose();
		}

	}
	/*
	[BurstCompile]
	private struct SpawnProjectileJob : IJobForEachWithEntity<ReadyToSpawnBulletComponent, LocalToWorld>
	{
		public EntityCommandBuffer.Concurrent CommandBuffer;
		//public NativeArray<BulletTemplateComponent> bulletTemplateArray;
		public Entity bulletEntity;

		public void Execute(Entity entity, int index, ref ReadyToSpawnBulletComponent spawner, ref LocalToWorld location)
		{
			var bullet = CommandBuffer.Instantiate(index, bulletEntity);
			CommandBuffer.g
			var bulletTemplateData = CommandBuffer.Get<BulletTemplateComponent>(bullet);
			var templateData =
			//bulletTagArray.
			//TODO: create new entity usingtemplate tag data
			//need to spawn from prefab so could be create or convert??? either way store prefab with bullet tag
			//Entity instance = CommandBuffer.CreateEntity(index);
			CommandBuffer.Instantiate(index, entity);
			BulletTemplateComponent bulletTemplate = bulletTemplateArray[0];
			Entity instance = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletTemplate.bulletPrefab, World.Active);
			//var instance = CommandBuffer.Instantiate(index, bulletEntity);

			var position = spawner.SpawnPosition;
			var bulletRotation = CalculateBulletDirection(spawner.Direction);
			CommandBuffer.SetComponent(index, instance, new Rotation { Value = bulletRotation });
			CommandBuffer.SetComponent(index, instance, new Translation { Value = position });
			CommandBuffer.AddComponent(index, instance, new ProjectileComponent
			{
				Speed = bulletTemplate.Speed,
				forward = spawner.Forward,
				lifeTime = bulletTemplate.LifeTime

			});
			CommandBuffer.AddComponent(index, instance, new DeadlyTag());

			CommandBuffer.DestroyEntity(index, entity);

		}

		private Quaternion CalculateBulletDirection(Quaternion playerRotation)
		{
			Vector3 euler = playerRotation.eulerAngles;
			Vector3 bulletRotation = new Vector3(0, euler.y + 90, 90);
			return Quaternion.Euler(bulletRotation);
		}
	}
	*/
}
