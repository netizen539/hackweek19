using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public class KeyboardInputSystem : BaseInputSystem
{
    protected override bool TryGetMovementDirection(out MovementDirection direction)
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




    protected override bool TryGetShield()
    {
        
        bool shieldActive = Input.GetKeyDown(KeyCode.Space);
        
        return shieldActive;
    }
}