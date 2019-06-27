using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using UnityEngine;

public abstract class BaseInputSystem : JobComponentSystem
{
	[BurstCompile]
	struct MovementInputSystemJob : IJobForEach<PlayerComponent, MovementComponent>
	{
		public float2 directionAxisPlayer;
#if UNITY_EDITOR
        public float2 directionAxisPlayer2;
#endif

        public float speed;

		public void Execute([ReadOnly] ref PlayerComponent player, ref MovementComponent movement)
		{
			movement.speed = speed;

            // if not moving, keep direction the same
            if (speed > 0)
            {
#if UNITY_EDITOR
                if (player.isPlayer1)
                    movement.playerDirectionAxis = directionAxisPlayer;
                else
                    movement.playerDirectionAxis = directionAxisPlayer2;
#else
                    movement.playerDirectionAxis = directionAxisPlayer;
#endif
            }

        }
	}

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
	{
		var job = new MovementInputSystemJob();

		float2 directionAxis;
#if UNITY_EDITOR
        float2 directionAxis2;
        if (TryGetMovementDirectionAxis(out directionAxis, out directionAxis2
#else
        if (TryGetMovementDirectionAxis(out directionAxis
#endif
            ))
        {
            job.directionAxisPlayer = directionAxis;
#if UNITY_EDITOR
            job.directionAxisPlayer2 = directionAxis2;
#endif
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
#if UNITY_EDITOR
    protected abstract bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis, out float2 player2DirectionAxis);
#else
    protected abstract bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis);
#endif
    protected abstract bool TryGetShield();
	protected abstract bool Fire();
}