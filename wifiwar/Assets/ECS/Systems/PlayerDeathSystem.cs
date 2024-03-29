﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

public class PlayerDeathSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		var deadPlayerQuery = EntityManager.CreateEntityQuery(typeof(HitByDeadlyComponent), typeof(PlayerComponent));

		using (var deadPlayers = deadPlayerQuery.ToEntityArray(Allocator.TempJob))
		{
			foreach (var deadPlayer in deadPlayers)
			{
				HitByDeadlyComponent hitByDeadlyComponent =
					EntityManager.GetComponentData<HitByDeadlyComponent>(deadPlayer);
				Entity deadlyEntity = hitByDeadlyComponent.DeadlyEntity;
				Entity attackingPlayer;
				uint attackerKills = 0;
				if (EntityManager.HasComponent<ProjectileComponent>(deadlyEntity))
				{
					var projectile = EntityManager.GetComponentData<ProjectileComponent>(deadlyEntity);
					attackingPlayer = projectile.Player;

					var attKills = EntityManager.GetComponentData<PlayerComponent>(attackingPlayer);
					attKills.kills++;
					attackerKills = attKills.kills;
					EntityManager.SetComponentData(attackingPlayer, attKills);
				}
				else
					attackingPlayer = deadlyEntity;

				Debug.Log("Player: " + attackingPlayer + "  killed " + deadPlayer);
				var deadKills = EntityManager.GetComponentData<PlayerComponent>(deadPlayer);
				Leaderboard.Current.RemoveRunningLeader((ulong)deadPlayer.Index);
				Leaderboard.Current.AddScore((ulong)deadPlayer.Index, deadKills.kills);
				Leaderboard.Current.UpdateRunningLeader((ulong)attackingPlayer.Index, attackerKills);
			}

			// actually kill players after full leaderboard update, in case player kills someone and dies at the same time
			foreach (var deadPlayer in deadPlayers)
			{
				EntityManager.RemoveComponent<PlayerComponent>(deadPlayer);
				var respawnData = EntityManager.GetComponentData<PlayerRespawnComponent>(deadPlayer);
				var playerTranslation = EntityManager.GetComponentData<Translation>(deadPlayer);
				respawnData.translation = playerTranslation.Value;
				playerTranslation.Value.x = -1000;
				EntityManager.SetComponentData(deadPlayer, playerTranslation);
				var movement = EntityManager.GetComponentData<MovementComponent>(deadPlayer);
				movement.playerDirectionAxis = new float2();
				EntityManager.SetComponentData(deadPlayer, respawnData);
				EntityManager.SetComponentData(deadPlayer, movement);
				EntityManager.SetComponentData(deadPlayer, new NewPowerUpComponent());
			}
		}
	}
}