using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class CrownConverter : MonoBehaviour, IConvertGameObjectToEntity
{

    public bool crownActive = false;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new CrownComponent() { crownActive = crownActive };
        dstManager.AddComponentData(entity, data);

    }
}
