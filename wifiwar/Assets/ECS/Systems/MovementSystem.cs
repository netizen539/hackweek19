using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using Unity.Physics;

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
    struct MovementSystemJob : IJobForEach<Translation, Rotation, MovementComponent, PhysicsVelocity, NewPowerUpComponent>
    {
        public float deltaTime;



        public void Execute(ref Translation translation, [ReadOnly] ref Rotation rotation,
            [ReadOnly] ref MovementComponent movement, [ReadOnly] ref PhysicsVelocity phyVelocity,
            [ReadOnly] ref NewPowerUpComponent powerUp)
        {
            float movementSpeed = movement.speed * deltaTime;

            float speedMultiplier = 1.0f;
            if (powerUp.powerType == PowerUpTypes.Speed)
                speedMultiplier = 3.0f;

            phyVelocity.Linear.x = 70 * movement.playerDirectionAxis.x * movementSpeed * speedMultiplier; 
            phyVelocity.Linear.z = 70 * movement.playerDirectionAxis.y * movementSpeed * speedMultiplier;

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