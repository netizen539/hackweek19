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
				Entity attackingPlayer = hitByDeadlyComponent.DeadlyEntity;
				Debug.Log("Player: " + attackingPlayer + "  killed " + deadPlayer);
				EntityManager.AddComponentData(deadPlayer, new DestroyTag());

				/*var attackerKills = EntityManager.GetComponentData<PlayerComponent>(attackingPlayer);
				attackerKills.kills++;
				EntityManager.SetComponentData(attackingPlayer, attackerKills);*/

				var deadKills = EntityManager.GetComponentData<PlayerComponent>(deadPlayer);
				Leaderboard.Current.AddScore(deadPlayer.ToString(), deadKills.kills);
			}
	}
}