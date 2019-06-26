using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using System;
using Unity.Rendering;
using Unity.Transforms;

[Serializable]
public struct PowerUpSystemComponent : IComponentData
{
    public float currentCharge;
    public float chargeTime;
}

public class PowerUpSystem : MonoBehaviour, IConvertGameObjectToEntity
{
    public float chargeTime = 5.0f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
       
        var data = new PowerUpSystemComponent
        {
            chargeTime = chargeTime
        };

        dstManager.AddComponentData(entity, data);
    }
}

public class PowerUpSystemBehavior : ComponentSystem
{
    protected override void OnUpdate()
    {
        BeginSimulationEntityCommandBufferSystem ecb;

        ecb = World.Active.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

  
        Entities.ForEach((Entity ent, ref PowerUpSystemComponent powerupsys) =>
        {
            

            if (powerupsys.currentCharge == powerupsys.chargeTime)
                return;

            var deltaTime = Time.deltaTime;
            powerupsys.currentCharge += deltaTime;

            if (powerupsys.currentCharge >= powerupsys.chargeTime)
            {
                powerupsys.currentCharge = powerupsys.chargeTime;

                var renderer = World.Active.EntityManager.GetSharedComponentData<RenderMesh>(ent);
                
                Vector2 offset = new Vector2(0.5f, 0.0f);
                renderer.material.mainTextureOffset = offset;

                PostUpdateCommands.SetSharedComponent<RenderMesh>(ent, renderer);
            }
        });
    }
}


