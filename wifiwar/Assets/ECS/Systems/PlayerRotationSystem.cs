using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Microsoft.CSharp;
using UnityEngine;

public class RotationSystem_just : JobComponentSystem
{
    private Unity.Entities.EntityQuery Player_Query;
    private float rotationAngle = 0.7854F;

    protected override void OnCreateManager()
    {
        Player_Query = GetEntityQuery(ComponentType.ReadWrite<Unity.Transforms.Rotation>(),
            ComponentType.ReadWrite<Unity.Transforms.Translation>(),
            ComponentType.ReadOnly<MovementComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        inputDeps.Complete();
        {
            var Player_QueryEntities = Player_Query.ToEntityArray(Allocator.TempJob);
            var Player_QueryRotationArray = Player_Query.ToComponentDataArray<Unity.Transforms.Rotation>(Allocator.TempJob);
            var Player_MovementSystemArray = Player_Query.ToComponentDataArray<MovementComponent>(Allocator.TempJob);

            for (int Player_QueryIdx = 0; Player_QueryIdx < Player_QueryEntities.Length; Player_QueryIdx++)
            {
                var Cube_QueryEntity = Player_QueryEntities[Player_QueryIdx];
                var Cube_QueryRotation = Player_QueryRotationArray[Player_QueryIdx];
                var Player_MovementSystem = Player_MovementSystemArray[Player_QueryIdx];

                Cube_QueryRotation.Value = quaternion.AxisAngle(new float3(0F, 1F, 0F), rotationAngle * (int)Player_MovementSystem.direction);
                
                EntityManager.SetComponentData<Unity.Transforms.Rotation>(Player_QueryEntities[Player_QueryIdx], Cube_QueryRotation);
            }
            
            Player_QueryRotationArray.Dispose();
            Player_QueryEntities.Dispose();
            Player_MovementSystemArray.Dispose();
        }

        return inputDeps;
    }
}
