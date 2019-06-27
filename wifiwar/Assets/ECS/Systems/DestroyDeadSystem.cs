using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class DestroyDeadSystem : JobComponentSystem
{
	EntityCommandBufferSystem m_Barrier;

	protected override void OnCreate()
	{
		m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	[BurstCompile]
	struct DestroyDeadPlayers : IJobForEachWithEntity<HitByDeadlyTag, PlayerComponent>
	{
		[WriteOnly]
		public EntityCommandBuffer.Concurrent CommandBuffer;

		public void Execute(Entity entity, int index, [ReadOnly] ref HitByDeadlyTag c0, [ReadOnly]ref PlayerComponent c1)
		{
			CommandBuffer.DestroyEntity(index, entity);
		}
	}

	[BurstCompile]
	struct DestroyDeadProjectiles : IJobForEachWithEntity<HitByDeadlyTag, ProjectileComponent>
	{
		[WriteOnly]
		public EntityCommandBuffer.Concurrent CommandBuffer;

		public void Execute(Entity entity, int index, [ReadOnly] ref HitByDeadlyTag c0, [ReadOnly]ref ProjectileComponent c1)
		{
			CommandBuffer.DestroyEntity(index, entity);
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

		var destroyProjectilesJob = new DestroyDeadProjectiles
		{
			CommandBuffer = commandBuffer,

		}.Schedule(this, inputDeps);

		var destroyPlayerJob = new DestroyDeadPlayers
		{
			CommandBuffer = commandBuffer,

		}.Schedule(this, destroyProjectilesJob);


		m_Barrier.AddJobHandleForProducer(destroyProjectilesJob);
		destroyProjectilesJob.Complete();
		m_Barrier.AddJobHandleForProducer(destroyPlayerJob);

		return destroyPlayerJob;
	}
}