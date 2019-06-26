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
    protected override bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis)
    {
        float2 directionAxis = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        playerDirectionAxis = directionAxis;
        return true;
    }




    protected override bool TryGetShield()
    {
        
        bool shieldActive = Input.GetKeyDown(KeyCode.Space);
        
        return shieldActive;
    }

    protected override bool Fire()
    {
        return Input.GetKeyDown(KeyCode.Return);
    }
}