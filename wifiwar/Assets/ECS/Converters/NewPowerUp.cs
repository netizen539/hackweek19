using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class NewPowerUp : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool none;
    public bool spreadshot;
    public bool speed;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new NewPowerUpComponent();

        if (none)
            data.powerType = PowerUpTypes.None;
        else if (spreadshot)
            data.powerType = PowerUpTypes.SpreadShot;
        else if (speed)
            data.powerType = PowerUpTypes.Speed;


        dstManager.AddComponentData(entity, data);

    }
}
