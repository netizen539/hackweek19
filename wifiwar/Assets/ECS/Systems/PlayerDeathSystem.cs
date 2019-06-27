using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public class PlayerDeathSystem : JobComponentSystem
{
	EntityCommandBufferSystem m_Barrier;

	protected override void OnCreate()
	{
		m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	struct PrintKillResults : IJobForEachWithEntity<HitByDeadlyComponent, PlayerComponent>
	{
		public void Execute(Entity entity, int index, [ReadOnly]ref HitByDeadlyComponent hitByDeadlyComponent, [ReadOnly]ref PlayerComponent playerComponent)
		{
			Entity attackingPlayer = hitByDeadlyComponent.DeadlyEntity;
			Entity deadPlayer = entity;

			Debug.Log("Player: " + attackingPlayer + "  killed " + deadPlayer);
		}
	}

	//[BurstCompile]
	struct MarkPlayerForDeath : IJobForEachWithEntity<HitByDeadlyComponent, PlayerComponent>
	{
		public EntityCommandBuffer.Concurrent CommandBuffer;

		public void Execute(Entity entity, int index, [ReadOnly]ref HitByDeadlyComponent hitByDeadlyComponent, [ReadOnly]ref PlayerComponent playerComponent)
		{
			CommandBuffer.AddComponent(index, entity, new DestroyTag());
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

		var printResultsJob = new PrintKillResults().Schedule(this, inputDeps);

		var markPlayerForDeathJob = new MarkPlayerForDeath
		{
			CommandBuffer = commandBuffer,

		}.Schedule(this, printResultsJob);


		m_Barrier.AddJobHandleForProducer(printResultsJob);
		//printResultsJob.Complete();
		m_Barrier.AddJobHandleForProducer(markPlayerForDeathJob);

		return markPlayerForDeathJob;
	}
}