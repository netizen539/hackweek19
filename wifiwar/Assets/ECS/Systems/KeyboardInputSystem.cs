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
#if UNITY_EDITOR
    protected override bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis, out float2 player2DirectionAxis)
#else
    protected override bool TryGetMovementDirectionAxis(out float2 playerDirectionAxis)
#endif
    {
        float2 directionAxis = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        playerDirectionAxis = directionAxis;

#if UNITY_EDITOR
        float2 directionAxis2 = new float2(Input.GetAxis("Horizontal1"), Input.GetAxis("Vertical1"));
        player2DirectionAxis = directionAxis2;
#endif
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

    protected override bool Respawn()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

#if UNITY_EDITOR
    protected override bool TryGetShield2()
    {

        bool shieldActive = Input.GetKeyDown(KeyCode.P);

        return shieldActive;
    }

    protected override bool Fire2()
    {
        return Input.GetKeyDown(KeyCode.LeftControl);
    }
#endif
}