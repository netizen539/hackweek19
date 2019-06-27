using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public abstract class BaseInputSystem : JobComponentSystem
{
	[BurstCompile]
	struct MovementInputSystemJob : IJobForEach<PlayerComponent, MovementComponent>
	{
		public float2 directionAxisPlayer;
		public float speed;

		public void Execute([ReadOnly] ref PlayerComponent player, ref MovementComponent movement)
		{
			movement.speed = speed;
			// if not moving, keep direction the same
			if (speed > 0)
				movement.playerDirectionAxis = directionAxisPlayer;
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDependencies)
	{
		var job = new MovementInputSystemJob();

		float2 directionAxis;
		if (TryGetMovementDirectionAxis(out directionAxis))
		{
			job.directionAxisPlayer = directionAxis;
			job.speed = MovementSystem.MaxSpeed;
		}
		else
		{
			job.speed = 0;
		}

		bool shieldAction = TryGetShield();
		bool fireAction = Fire();
		if (shieldAction)
		{
			var shieldQuery = EntityManager.CreateEntityQuery(typeof(ShieldComponent));
			using (var shields = shieldQuery.ToEntityArray(Allocator.TempJob))
				foreach (var sh in shields)
				{
					var shield = EntityManager.GetComponentData<ShieldComponent>(sh);
					shield.shieldOn = !shield.shieldOn;
					EntityManager.SetComponentData(sh, shield);
				}
		}

		if (fireAction)
		{
			var playerQuery = EntityManager.CreateEntityQuery(typeof(PlayerComponent));
			using (var players = playerQuery.ToEntityArray(Allocator.TempJob))
				foreach (var e in players)
					EntityManager.AddComponentData(e, new ReadyToSpawnBulletComponent());
		}

		return job.Schedule(this, inputDependencies);
	}

	protected abstract bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis);
	protected abstract bool TryGetShield();
	protected abstract bool Fire();
}