using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class RemoveDestroyedSystem : JobComponentSystem
{
	EntityCommandBufferSystem m_Barrier;

	protected override void OnCreate()
	{
		m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	//[BurstCompile]
	struct RemoveDestroyedEntities : IJobForEachWithEntity<DestroyTag>
	{
		[WriteOnly]
		public EntityCommandBuffer.Concurrent CommandBuffer;

		public void Execute(Entity entity, int index, [ReadOnly] ref DestroyTag c0)
		{
			CommandBuffer.DestroyEntity(index, entity);
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

		var removeJob = new RemoveDestroyedEntities
		{
			CommandBuffer = commandBuffer,

		}.Schedule(this, inputDeps);

		m_Barrier.AddJobHandleForProducer(removeJob);

		return removeJob;
	}
}
