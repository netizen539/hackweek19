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
public struct TriggerTestComponent : IComponentData
{
    public bool enabled;
}

public class TriggerTestAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    void OnEnable() { }

    void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (enabled)
        {
            dstManager.AddComponentData(entity, new TriggerTestComponent()
            {
                enabled = true
            });
        }
    }
}

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class TriggerTestSystem : JobComponentSystem
{
    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;

    EntityQuery TriggerGroup;

    BeginSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.Active.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        TriggerGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(TriggerTestComponent), }
        });
    }

   // [BurstCompile]
    struct TriggerGravityFactorJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<TriggerTestComponent> TriggerTestGroup;
        //[ReadOnly] public ComponentDataFromEntity<TriggerTestComponent> TriggerTestGroup;
        public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;
        public EntityCommandBuffer CommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.Entities.EntityA;
            Entity entityB = triggerEvent.Entities.EntityB;

            bool isBodyATrigger = TriggerTestGroup.Exists(entityA);
            bool isBodyBTrigger = TriggerTestGroup.Exists(entityB);

            // Ignoring Triggers overlapping other Triggers
            if (isBodyATrigger && isBodyBTrigger)
                return;

            bool isBodyADynamic = PhysicsVelocityGroup.Exists(entityA);
            bool isBodyBDynamic = PhysicsVelocityGroup.Exists(entityB);

            // Ignoring overlapping static bodies
            if ((isBodyATrigger && !isBodyBDynamic) ||
                (isBodyBTrigger && !isBodyADynamic))
                return;

            var triggerEntity = isBodyATrigger ? entityA : entityB;
            var dynamicEntity = isBodyATrigger ? entityB : entityA;

            var triggerEnt = TriggerTestGroup[triggerEntity];

            if (triggerEnt.enabled)
            {
                CommandBuffer.AddComponent<GotSword>(dynamicEntity, new GotSword());
                CommandBuffer.AddComponent<PlayerHit>(triggerEntity, new PlayerHit());
                triggerEnt.enabled = false;
                TriggerTestGroup[triggerEntity] = triggerEnt;
            }
                

            // tweak PhysicsGravityFactor
            //{
            //    var component = PhysicsGravityFactorGroup[dynamicEntity];
            //    component.Value = triggerGravityComponent.remove;
            //    PhysicsGravityFactorGroup[dynamicEntity] = component;
            //}
            //// damp velocity
            //{
            //    var component = PhysicsVelocityGroup[dynamicEntity];
            //    component.Linear *= triggerGravityComponent.remove2;
            //    PhysicsVelocityGroup[dynamicEntity] = component;
            //}
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        
        JobHandle jobHandle = new TriggerGravityFactorJob
        {
            TriggerTestGroup = GetComponentDataFromEntity<TriggerTestComponent>(false),
            PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
            CommandBuffer = ecbSystem.CreateCommandBuffer()
        }.Schedule(m_StepPhysicsWorldSystem.Simulation,
                    ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
