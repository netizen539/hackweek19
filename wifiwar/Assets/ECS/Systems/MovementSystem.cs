using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class MovementSystem : JobComponentSystem
{
    public static readonly float2[] directions = new float2[] {
        new float2(0, 1),
        new float2(1, 1),
        new float2(1, 0),
        new float2(1, -1),
        new float2(0, -1),
        new float2(-1, -1),
        new float2(-1, 0),
        new float2(-1, 1), 
    };

    [BurstCompile]
    struct MovementSystemJob : IJobForEach<Translation, Rotation, MovementComponent>
    {
        public float deltaTime;
        
        
        
        public void Execute(ref Translation translation, [ReadOnly] ref Rotation rotation, [ReadOnly] ref MovementComponent movement)
        {
            float movementSpeed = movement.speed * deltaTime;
            switch (movement.direction)
            {
                case MovementDirection.UP_RIGHT:
                case MovementDirection.DOWN_RIGHT:
                case MovementDirection.DOWN_LEFT:
                case MovementDirection.UP_LEFT:
                {
                    movementSpeed /= sqrt(2.0f);
                    break;
                }
            }
            var movementVec = directions[(int) movement.direction];
            movementVec.x = movementVec.x * movementSpeed + translation.Value.x;
            movementVec.y = movementVec.y * movementSpeed + translation.Value.z;
            translation.Value = new float3(movementVec.x, 0, movementVec.y);
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new MovementSystemJob();
        
        job.deltaTime = UnityEngine.Time.deltaTime;
        
        
        
        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(this, inputDependencies);
    }
}