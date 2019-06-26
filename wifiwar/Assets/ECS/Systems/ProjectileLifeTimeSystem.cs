using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class ProjectileLifeTimeSystem : JobComponentSystem
{
	EntityCommandBufferSystem m_Barrier;

	protected override void OnCreate()
	{
		m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	[BurstCompile]
	struct ProjectileLifeTimeSystemJob : IJobForEachWithEntity<ProjectileComponent>
	{
		public float deltaTime;
		[WriteOnly]
		public EntityCommandBuffer.Concurrent CommandBuffer;

		public void Execute(Entity entity, int jobIndex, ref ProjectileComponent projectileComponent)
		{
			projectileComponent.lifeTime -= deltaTime;

			if (projectileComponent.lifeTime < 0.0f)
			{
				CommandBuffer.DestroyEntity(jobIndex, entity);
			}
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

		var job = new ProjectileLifeTimeSystemJob
		{
			deltaTime = Time.deltaTime,
			CommandBuffer = commandBuffer

		}.Schedule(this, inputDeps);

		m_Barrier.AddJobHandleForProducer(job);
		return job;
	}
}