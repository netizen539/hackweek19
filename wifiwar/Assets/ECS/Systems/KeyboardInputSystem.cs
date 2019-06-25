using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public class KeyboardInputSystem : JobComponentSystem
{
    // This declares a new kind of job, which is a unit of work to do.
    // The job is declared as an IJobForEach<Translation, Rotation>,
    // meaning it will process all entities in the world that have both
    // Translation and Rotation components. Change it to process the component
    // types you want.
    //
    // The job is also tagged with the BurstCompile attribute, which means
    // that the Burst compiler will optimize it for the best performance.
    [BurstCompile]
    struct KeyboardInputSystemJob : IJobForEach<MovementComponent>
    {
        public MovementDirection direction;
        public float speed;

        public void Execute(ref MovementComponent movement)
        {
            movement.direction = direction;
            movement.speed = speed;
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new KeyboardInputSystemJob();
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
        return job.Schedule(this, inputDependencies);
    }
    
    bool TryGetMovementDirection(out MovementDirection direction)
    {
        direction = MovementDirection.UP;
        
        bool up = Input.GetKey(KeyCode.UpArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);

        // Invalid moves
        if (!(up || down || left || right))
            return false;
        if (up && down || left && right)
            return false;

        if (up)
        {
            if (left)
                direction = MovementDirection.UP_LEFT;
            else if (right)
                direction = MovementDirection.UP_RIGHT;
            else
                direction = MovementDirection.UP;
        }
        else if (down)
        {
            if (left)
                direction = MovementDirection.DOWN_LEFT;
            else if (right)
                direction = MovementDirection.DOWN_RIGHT;
            else
                direction = MovementDirection.DOWN;
        }
        else if (left)
            direction = MovementDirection.LEFT;
        else
            direction = MovementDirection.RIGHT;

        return true;
    }
}