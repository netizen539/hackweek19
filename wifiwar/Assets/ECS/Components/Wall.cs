using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class Wall : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new WallTag { };
        dstManager.AddComponentData(entity, data);
    }
}
