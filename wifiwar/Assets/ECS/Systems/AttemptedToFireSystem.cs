using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public class AttemptedToFireSystem : JobComponentSystem
{
	EntityCommandBufferSystem m_Barrier;

	protected override void OnCreate()
	{
		m_Barrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

		var cooldownJob = new DecreaseFireCooldown
		{
			deltaTime = Time.deltaTime,

		}.Schedule(this, inputDeps);

		var setReadyToFireJob = new SetReadyToFire
		{
			CommandBuffer = commandBuffer

		}.Schedule(this, cooldownJob);

		m_Barrier.AddJobHandleForProducer(cooldownJob);
		return setReadyToFireJob;
	}

	[BurstCompile]
	struct DecreaseFireCooldown : IJobForEachWithEntity<PlayerComponent>
	{
		public float deltaTime;

		public void Execute(Entity entity, int index, ref PlayerComponent playerComponent)
		{
			playerComponent.currentCoolDown -= deltaTime;
			playerComponent.currentCoolDown = math.max(0f, playerComponent.currentCoolDown);
		}
	}

	//[BurstCompile]
	struct SetReadyToFire : IJobForEachWithEntity<PlayerComponent, AttemptedToFireTag>
	{
		[WriteOnly]
		public EntityCommandBuffer.Concurrent CommandBuffer;

		public void Execute(Entity entity, int index, ref PlayerComponent playerComponent, [ReadOnly]ref AttemptedToFireTag readyToFire)
		{
			if (playerComponent.currentCoolDown == 0)
			{
				CommandBuffer.AddComponent(index, entity, new ReadyToSpawnBulletComponent());
				CommandBuffer.RemoveComponent<AttemptedToFireTag>(index, entity);
				playerComponent.currentCoolDown = playerComponent.MaxCooldown;


			}
		}
	}


}