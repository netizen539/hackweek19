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
    struct MovementInputSystemJobPlayer1 : IJobForEach<PlayerComponent, Player1_tag, MovementComponent>
    {
        public float2 directionAxisPlayer;

        public float speed;

        public void Execute([ReadOnly] ref PlayerComponent p, [ReadOnly] ref Player1_tag player, ref MovementComponent movement)
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
    struct MovementInputSystemJobPlayer2 : IJobForEach<PlayerComponent, Player2_tag, MovementComponent>
    {
        public float2 directionAxisPlayer;

        public float speed;

        public void Execute([ReadOnly] ref PlayerComponent p, [ReadOnly] ref Player2_tag player, ref MovementComponent movement)
        {
            movement.speed = speed;

            // if not moving, keep direction the same
            if (speed > 0)
            {
                movement.playerDirectionAxis = directionAxisPlayer;
            }

        }
    }

    // Dummy job, because if all players die, other jobs no longer run and this system does not run
    struct InputSystemJob : IJobForEach<Player1_tag>
    {
	    public void Execute([ReadOnly] ref Player1_tag player)
	    {}
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
			var shieldQuery = EntityManager.CreateEntityQuery(typeof(ShieldComponent), typeof(Player1_tag));
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
			var playerQuery = EntityManager.CreateEntityQuery(typeof(PlayerComponent), typeof(Player1_tag));
            using (var players = playerQuery.ToEntityArray(Allocator.TempJob))
                foreach (var e in players)
                    EntityManager.AddComponentData(e, new ReadyToSpawnBulletComponent());
		}
#if UNITY_EDITOR
        bool fireActionP2 = Fire2();
        bool shieldAction2 = TryGetShield2();

        if (shieldAction2)
        {
            var shieldQuery = EntityManager.CreateEntityQuery(typeof(ShieldComponent), typeof(Player2_tag));
            using (var shields = shieldQuery.ToEntityArray(Allocator.TempJob))
                foreach (var sh in shields)
                {
                    var shield = EntityManager.GetComponentData<ShieldComponent>(sh);
                    shield.shieldOn = !shield.shieldOn;
                    EntityManager.SetComponentData(sh, shield);
                }
        }

        if (fireActionP2)
        {
            var playerQuery = EntityManager.CreateEntityQuery(typeof(PlayerComponent), typeof(Player2_tag));
            using (var players = playerQuery.ToEntityArray(Allocator.TempJob))
                foreach (var e in players)
                    EntityManager.AddComponentData(e, new ReadyToSpawnBulletComponent());
        }
#endif
        if (Respawn())
		{
			var deadPlayerQuery = EntityManager.CreateEntityQuery(typeof(HitByDeadlyComponent));
			using (var deadPlayers = deadPlayerQuery.ToEntityArray(Allocator.TempJob))
				foreach (var player in deadPlayers)
				{
					if (!EntityManager.HasComponent<PlayerComponent>(player))
					{
                        EntityManager.AddComponentData(player, new RespawnRequest());
						break;
					}
				}
		}

		var dummy = new InputSystemJob();
        return dummy.Schedule(this,job2.Schedule(this, job1.Schedule(this, inputDependencies)));
    }
#if UNITY_EDITOR
    protected abstract bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis, out float2 player2DirectionAxis);
    protected abstract bool TryGetShield2();
    protected abstract bool Fire2();
#else
    protected abstract bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis);
#endif
    protected abstract bool TryGetShield();
	protected abstract bool Fire();
	protected abstract bool Respawn();
}