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
        //movement.direction = MovementDirection.UP_LEFT;
        movement.speed = speed;
        dstManager.AddComponentData(entity, movement);
    }
}
