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
    [BurstCompile]
    struct PlayerRotationSystemJob : IJobForEach<Rotation, MovementComponent>
    {
        public void Execute(ref Rotation rotation, [ReadOnly] ref MovementComponent movement)
        {
            float rotationAngle = 0.7854F;
            rotation.Value = quaternion.AxisAngle(new float3(0F, 1F, 0F), rotationAngle * ((int)movement.direction));
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new PlayerRotationSystemJob();
        
        return job.Schedule(this, inputDependencies);
    }
}
