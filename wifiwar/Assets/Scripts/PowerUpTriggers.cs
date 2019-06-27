using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Collider = Unity.Physics.Collider;
using Unity.Transforms;
using Unity.Profiling;
using Unity.Burst;
using System;
using UnityEditor;
using Unity.Collections.LowLevel.Unsafe;

[Serializable]
public struct PowerUpTriggerComponent : IComponentData
{
    public bool enabled;
    public bool isSwordPowerUp;
    public bool isGunPowerUp;
    public bool isSpreadshot;
    public bool isSpeed;
    public bool isDeadly;
}

public class PowerUpTriggers : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool isSwordPowerUp;
    public bool isGunPowerUp;
    public bool isSpreadShot;
    public bool isSpeed;
    public bool isDeadly;

    void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (enabled)
        {
            dstManager.AddComponentData(entity, new PowerUpTriggerComponent()
            {
                enabled = true,
                isGunPowerUp = isGunPowerUp,
                isSwordPowerUp = isSwordPowerUp,
                isDeadly = isDeadly,
                isSpreadshot = isSpreadShot,
                isSpeed = isSpeed
            });
        }
    }

    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class PowerUpTriggerSystem : JobComponentSystem
    {
        BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        StepPhysicsWorld m_StepPhysicsWorldSystem;
        EntityQuery TriggerGroupQuery;
        BeginSimulationEntityCommandBufferSystem ecbSystem;
        EntityCommandBuffer commandBuffer;
        

        protected override void OnCreate()
        {
            ecbSystem = World.Active.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
            TriggerGroupQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(PowerUpTriggerComponent), }
            });

            commandBuffer = ecbSystem.CreateCommandBuffer();
        }

        // [BurstCompile]
        struct PowerUpTriggerJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<PowerUpTriggerComponent> PowerUpTriggerGroup;
            public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;
            [ReadOnly]
            public ComponentDataFromEntity<WallTag> WallGroup;
            [ReadOnly]
            public ComponentDataFromEntity<DestroyTag> DestroyGroup;
            [ReadOnly]
            public ComponentDataFromEntity<HitByDeadlyComponent> HitByDeadlyGroup;
            public EntityCommandBuffer CommandBuffer;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.Entities.EntityA;
                Entity entityB = triggerEvent.Entities.EntityB;

                bool isBodyATrigger = PowerUpTriggerGroup.Exists(entityA);
                bool isBodyBTrigger = PowerUpTriggerGroup.Exists(entityB);

                // Ignoring Triggers overlapping other Triggers
                if (isBodyATrigger && isBodyBTrigger)
                    return;

                bool isBodyADynamic = PhysicsVelocityGroup.Exists(entityA);
                bool isBodyBDynamic = PhysicsVelocityGroup.Exists(entityB);

                var triggerEntity = isBodyATrigger ? entityA : entityB; 
                var dynamicEntity = isBodyATrigger ? entityB : entityA;

                bool isTriggerPowerUp = PowerUpTriggerGroup.Exists(triggerEntity);
                if (!isTriggerPowerUp)
                    return;

                if (DestroyGroup.Exists(triggerEntity))
                    return;

                bool isWall = WallGroup.Exists(triggerEntity);
                var powerUpComponent = PowerUpTriggerGroup[triggerEntity];

                if(powerUpComponent.isDeadly && isWall)
                {
                    CommandBuffer.AddComponent<DestroyTag>(triggerEntity, new DestroyTag());
                }
                else if(powerUpComponent.isDeadly)
                {
                    CommandBuffer.AddComponent<DestroyTag>(triggerEntity, new DestroyTag());
                    if(!HitByDeadlyGroup.Exists(dynamicEntity))
                        CommandBuffer.AddComponent<HitByDeadlyComponent>(dynamicEntity, new HitByDeadlyComponent { DeadlyEntity = triggerEntity });
                }
                else if (powerUpComponent.enabled)
                {
                    CommandBuffer.AddComponent<PlayerHit>(triggerEntity, new PlayerHit());
                    powerUpComponent.enabled = false;

                    if (powerUpComponent.isSwordPowerUp)
                        CommandBuffer.AddComponent<GotSword>(dynamicEntity, new GotSword());
                    else if(powerUpComponent.isGunPowerUp)
                        CommandBuffer.AddComponent<GotGun>(dynamicEntity, new GotGun());
                    else if (powerUpComponent.isSpeed)
                        CommandBuffer.AddComponent<PowerUpSpeedTag>(dynamicEntity, new PowerUpSpeedTag());
                    else if (powerUpComponent.isSpreadshot)
                        CommandBuffer.AddComponent<GotGun>(dynamicEntity, new GotGun());


                    PowerUpTriggerGroup[triggerEntity] = powerUpComponent;
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            JobHandle jobHandle = new PowerUpTriggerJob
            {
                PowerUpTriggerGroup = GetComponentDataFromEntity<PowerUpTriggerComponent>(false),
                PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
                WallGroup = GetComponentDataFromEntity<WallTag>(true),
                DestroyGroup = GetComponentDataFromEntity<DestroyTag>(true),
                HitByDeadlyGroup = GetComponentDataFromEntity<HitByDeadlyComponent>(true),
                CommandBuffer = ecbSystem.CreateCommandBuffer()
            }.Schedule(m_StepPhysicsWorldSystem.Simulation,
                        ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);

            ecbSystem.AddJobHandleForProducer(jobHandle);
            return jobHandle;
        }
    }
}
