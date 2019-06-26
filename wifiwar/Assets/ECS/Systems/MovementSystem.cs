using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class MovementSystem : JobComponentSystem
{
    private static float maxSpeed;
    
    public static float MaxSpeed
    {
        get { return maxSpeed; }
    }

    public static void SetMaxSpeed(float speed)
    {
        maxSpeed = speed;
    }
    
    [BurstCompile]
    struct MovementSystemJob : IJobForEach<Translation, Rotation, MovementComponent>
    {
        public float deltaTime;
        
        
        
        public void Execute(ref Translation translation, [ReadOnly] ref Rotation rotation, [ReadOnly] ref MovementComponent movement)
        {
            float movementSpeed = movement.speed * deltaTime;
            translation.Value.x += movement.playerDirectionAxis.x * movementSpeed;
            translation.Value.z += movement.playerDirectionAxis.y * movementSpeed;

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