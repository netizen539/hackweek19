using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class MovementConvert : MonoBehaviour, IConvertGameObjectToEntity
{
    public float speed;
    
    

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var movement = new MovementComponent();
        dstManager.AddComponentData(entity, movement);
        MovementSystem.SetMaxSpeed(speed);
    }
}
