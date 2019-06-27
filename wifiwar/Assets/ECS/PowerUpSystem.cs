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

[Serializable]
public struct GotSword : IComponentData{}
public struct GotGun : IComponentData { }
public struct PlayerHit : IComponentData
{
    int wtf;
}

public class PowerUpSystem : MonoBehaviour, IConvertGameObjectToEntity
{
    public float chargeTime = 5.0f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        var data = new PowerUpSystemComponent
        {
            chargeTime = chargeTime,
            currentCharge = 0
        };

        dstManager.AddComponentData(entity, data);
    }
}

public class PowerUpSystemBehavior : ComponentSystem
{
    public EntityCommandBuffer commandBuffer;
    public BeginSimulationEntityCommandBufferSystem ecb;

    protected override void OnCreate()
    {
        ecb = World.Active.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        commandBuffer = ecb.CreateCommandBuffer();

        Entities.ForEach((Entity ent, ref PlayerHit playerHit, ref PowerUpSystemComponent powerup) =>
        {
        //    commandBuffer.RemoveComponent<PlayerHit>(ent);
              powerup.currentCharge = 0;
        });


        Entities.ForEach((Entity ent, ref PowerUpSystemComponent powerupsys, ref PowerUpTriggerComponent powerUpTriggerComponent) =>
        {
            

            if (powerupsys.currentCharge == powerupsys.chargeTime)
                return;

            if (powerupsys.currentCharge == 0)
            {
  
                powerUpTriggerComponent.enabled = false;

                var renderer = World.Active.EntityManager.GetSharedComponentData<RenderMesh>(ent);

                Vector2 offset = new Vector2(0.5f, 0.0f);
                renderer.material.mainTextureOffset = offset;

                PostUpdateCommands.SetSharedComponent<RenderMesh>(ent, renderer);
            }

            var deltaTime = Time.deltaTime;
            powerupsys.currentCharge += deltaTime;

            if (powerupsys.currentCharge >= powerupsys.chargeTime)
            {
                powerupsys.currentCharge = powerupsys.chargeTime;
                powerUpTriggerComponent.enabled = true;

                var renderer = World.Active.EntityManager.GetSharedComponentData<RenderMesh>(ent);
                
                Vector2 offset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureOffset = offset;

                PostUpdateCommands.SetSharedComponent<RenderMesh>(ent, renderer);
            }

        });
    }
}


