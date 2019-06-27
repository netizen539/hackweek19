using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using UnityEngine;
using float3 = Unity.Mathematics.float3;

public abstract class BaseInputSystem : JobComponentSystem
{
    [BurstCompile]
    struct MovementInputSystemJobPlayer1 : IJobForEach<Player1_tag, MovementComponent>
    {
        public float2 directionAxisPlayer;

        public float speed;

        public void Execute([ReadOnly] ref Player1_tag player, ref MovementComponent movement)
        {
            movement.speed = speed;

            // if not moving, keep direction the same
            if (speed > 0)
            {
                movement.playerDirectionAxis = directionAxisPlayer;
            }

        }
    }

    [BurstCompile]
    struct MovementInputSystemJobPlayer2 : IJobForEach<Player2_tag, MovementComponent>
    {
        public float2 directionAxisPlayer;

        public float speed;

        public void Execute([ReadOnly] ref Player2_tag player, ref MovementComponent movement)
        {
            movement.speed = speed;

            // if not moving, keep direction the same
            if (speed > 0)
            {
                movement.playerDirectionAxis = directionAxisPlayer;
            }

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job1 = new MovementInputSystemJobPlayer1();
        var job2 = new MovementInputSystemJobPlayer2();

        float2 directionAxis;
#if UNITY_EDITOR
        float2 directionAxis2;
        if (TryGetMovementDirectionAxis(out directionAxis, out directionAxis2
#else
        if (TryGetMovementDirectionAxis(out directionAxis
#endif
            ))
        {
            job1.directionAxisPlayer = directionAxis;
#if UNITY_EDITOR
            job2.directionAxisPlayer = directionAxis2;
#endif
            job1.speed = MovementSystem.MaxSpeed;
            job2.speed = MovementSystem.MaxSpeed;
        }
        else
        {
            job1.speed = 0;
            job2.speed = 0;
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

		if (Respawn())
		{
			var deadPlayerQuery = EntityManager.CreateEntityQuery(typeof(HitByDeadlyComponent));
			using (var deadPlayers = deadPlayerQuery.ToEntityArray(Allocator.TempJob))
				foreach (var player in deadPlayers)
				{
					if (!EntityManager.HasComponent<PlayerComponent>(player))
					{
						EntityManager.RemoveComponent<HitByDeadlyComponent>(player);
						EntityManager.AddComponentData(player, new PlayerComponent { kills = 0 });
						break;
					}
				}
		}

        return job2.Schedule(this, job1.Schedule(this, inputDependencies));
    }
#if UNITY_EDITOR
    protected abstract bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis, out float2 player2DirectionAxis);
#else
    protected abstract bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis);
#endif
    protected abstract bool TryGetShield();
	protected abstract bool Fire();
	protected abstract bool Respawn();
}