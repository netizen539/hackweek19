using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public abstract class BaseInputSystem : JobComponentSystem
{
    [BurstCompile]
    struct MovementInputSystemJob : IJobForEach<MovementComponent>
    {
        public MovementDirection direction;
        public float speed;

        public void Execute(ref MovementComponent movement)
        {
            movement.speed = speed;
            // if not moving, keep direction the same
            if (speed > 0)
                movement.direction = direction;
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new MovementInputSystemJob();
        MovementDirection direction;
        if (TryGetMovementDirection(out direction))
        {
            job.direction = direction;
            job.speed = MovementSystem.MaxSpeed;
        }
        else
        {
            job.speed = 0;
        }

        if (TryGetShield())
            foreach (var e in EntityManager.GetAllEntities())
            {
                if (EntityManager.HasComponent<ShieldComponent>(e))
                {
                   var shield = EntityManager.GetComponentData<ShieldComponent>(e);
                    shield.shieldOn = !shield.shieldOn;
                    EntityManager.SetComponentData(e, shield);
                }

            }

        return job.Schedule(this, inputDependencies);
    }

    protected abstract bool TryGetMovementDirection(out MovementDirection direction);
    protected abstract bool TryGetShield();
}