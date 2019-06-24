using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotationSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        /*
         *         Entities.ForEach((ref RotationSpeed_ForEach rotationSpeed, ref Rotation rotation) =>
        {
            var deltaTime = Time.deltaTime;
            rotation.Value = math.mul(math.normalize(rotation.Value),
                quaternion.AxisAngle(math.up(), rotationSpeed.RadiansPerSecond * deltaTime));
        });
         */
        Entities.ForEach((ref RotationSpeedComponent rotationSpeed, ref Rotation rotation) =>
        {
            float deltaTime = Time.deltaTime;
            rotation.Value = math.mul(math.normalize(rotation.Value),
                Quaternion.AngleAxis(rotationSpeed.rotationSpeed * deltaTime, math.up()));
        });
    }
}
