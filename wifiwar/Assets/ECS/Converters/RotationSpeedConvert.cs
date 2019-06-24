using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RotationSpeedConvert : MonoBehaviour, IConvertGameObjectToEntity
{
    public float rotationSpeed = 45.0f;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new RotationSpeedComponent() { rotationSpeed = math.radians(rotationSpeed) };
        dstManager.AddComponentData(entity, data);
    }
}
