using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class PlayerRespawnSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var deadPlayerQuery = EntityManager.CreateEntityQuery(typeof(HitByDeadlyComponent), typeof(RespawnRequest));
        using (var deadPlayers = deadPlayerQuery.ToEntityArray(Allocator.TempJob))
            foreach (var player in deadPlayers)
            {
                if (!EntityManager.HasComponent<PlayerComponent>(player))
                {
                    EntityManager.RemoveComponent<HitByDeadlyComponent>(player);
                    EntityManager.RemoveComponent<RespawnRequest>(player);
                    EntityManager.AddComponentData(player, new PlayerComponent { kills = 0 });
                    var respawnData = EntityManager.GetComponentData<PlayerRespawnComponent>(player);
                    EntityManager.SetComponentData(player, new Translation { Value = respawnData.translation });
                }
            }
    }
}