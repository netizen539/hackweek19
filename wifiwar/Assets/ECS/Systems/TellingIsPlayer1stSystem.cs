using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using Unity.Physics;

public class RotationSpeedSystem : ComponentSystem
{
    protected override void OnStartRunning()
    {
#if UNITY_EDITOR
        int i = 0;
        Entities.ForEach((ref PlayerComponent player) =>
        {
            if (i == 0)
                player.isPlayer1 = true;
            else
                player.isPlayer1 = false;
            i++;
        });
#endif

    }
    protected override void OnUpdate()
    {
    }
}
