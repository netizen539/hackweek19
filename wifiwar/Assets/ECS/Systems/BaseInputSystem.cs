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
		//public MovementDirection direction;
		public float2 directionAxisPlayer;
		public float speed;

		public void Execute([ReadOnly] ref PlayerComponent player, ref MovementComponent movement)
		{
			movement.speed = speed;
			// if not moving, keep direction the same
			if (speed > 0)
				// movement.direction = direction;
				movement.playerDirectionAxis = directionAxisPlayer;
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDependencies)
	{
		var job = new MovementInputSystemJob();
		/*MovementDirection direction;
        if (TryGetMovementDirection(out direction))
        {
            job.direction = direction;
            job.speed = MovementSystem.MaxSpeed;
        }*/

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
		if (shieldAction || fireAction)
		{
			var query = EntityManager.CreateEntityQuery(typeof(PlayerComponent));
			using (var players = query.ToEntityArray(Allocator.TempJob))
				foreach (var e in players)
				{
					if (shieldAction)
					{
						var shield = EntityManager.GetComponentData<ShieldComponent>(e);
						shield.shieldOn = !shield.shieldOn;
						EntityManager.SetComponentData(e, shield);
					}

					if (fireAction)
					{
						// TODO
						EntityManager.AddComponentData(e, new ReadyToSpawnBulletComponent());
					}
				}
		}

		return job.Schedule(this, inputDependencies);
	}

	//protected abstract bool TryGetMovementDirection(out MovementDirection direction);
	protected abstract bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis);
	protected abstract bool TryGetShield();
	protected abstract bool Fire();
}