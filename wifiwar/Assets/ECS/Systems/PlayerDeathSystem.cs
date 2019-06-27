using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public class PlayerDeathSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		var deadPlayerQuery = EntityManager.CreateEntityQuery(typeof(HitByDeadlyComponent), typeof(PlayerComponent));

		using (var deadPlayers = deadPlayerQuery.ToEntityArray(Allocator.TempJob))
			foreach (var deadPlayer in deadPlayers)
			{
				HitByDeadlyComponent hitByDeadlyComponent =
					EntityManager.GetComponentData<HitByDeadlyComponent>(deadPlayer);
				Entity deadlyEntity = hitByDeadlyComponent.DeadlyEntity;
				Entity attackingPlayer;
				if (EntityManager.HasComponent<ProjectileComponent>(deadlyEntity))
				{
					var projectile = EntityManager.GetComponentData<ProjectileComponent>(deadlyEntity);
					attackingPlayer = projectile.Player;

					var attackerKills = EntityManager.GetComponentData<PlayerComponent>(attackingPlayer);
					attackerKills.kills++;
					EntityManager.SetComponentData(attackingPlayer, attackerKills);
				}
				else
					attackingPlayer = deadlyEntity;
				Debug.Log("Player: " + attackingPlayer + "  killed " + deadPlayer);
				var deadKills = EntityManager.GetComponentData<PlayerComponent>(deadPlayer);
				Leaderboard.Current.AddScore(deadPlayer.ToString(), deadKills.kills);
				
				EntityManager.RemoveComponent<PlayerComponent>(deadPlayer);
				var playerTranslation = EntityManager.GetComponentData<Translation>(deadPlayer);
				playerTranslation.Value.x = 0;
				playerTranslation.Value.z = 0;
				EntityManager.SetComponentData(deadPlayer, playerTranslation);
			}
	}
}