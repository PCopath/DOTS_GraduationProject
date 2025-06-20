using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public struct PlasmaBlastData : IComponentData
{
    public float MoveSpeed;
    public int AttackDamage;
}

public struct PlasmaBlastExpirationTimer : IComponentData
{
    public float Value;
}

public class PlasmaBlastAuthoring : MonoBehaviour
{
    public float MoveSpeed;
    public int AttackDamage;

    public float DestroyAfterTime;

    private class Baker : Baker<PlasmaBlastAuthoring>
    {
        public override void Bake(PlasmaBlastAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlasmaBlastData
            {
                MoveSpeed = authoring.MoveSpeed,
                AttackDamage = authoring.AttackDamage
            });

            AddComponent(entity, new PlasmaBlastExpirationTimer
            {
                Value = authoring.DestroyAfterTime
            });

            AddComponent<DestroyEntityFlag>(entity);
            SetComponentEnabled<DestroyEntityFlag>(entity, false);
        }
    }
}

public partial struct MovePlasmaBlastSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (transform, data) in SystemAPI.Query<RefRW<LocalTransform>, PlasmaBlastData>())
        {
            transform.ValueRW.Position += transform.ValueRO.Right() * data.MoveSpeed * deltaTime;
        }

        // Destroy Plasma Blast After Time
        foreach (var (timer, entity) in SystemAPI.Query<RefRW<PlasmaBlastExpirationTimer>>().WithPresent<DestroyEntityFlag>().WithEntityAccess())
        {
            timer.ValueRW.Value -= deltaTime;
            if (timer.ValueRO.Value > 0) continue;
            SystemAPI.SetComponentEnabled<DestroyEntityFlag>(entity, true);
        }
    }
}

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[UpdateBefore(typeof(AfterPhysicsSystemGroup))]
public partial struct PlasmaBlastAttackSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var attackJob = new PlasmaBlastAttackJob
        {
            PlasmaBlastLookup = SystemAPI.GetComponentLookup<PlasmaBlastData>(true),
            EnemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true),
            DamageBufferLookup = SystemAPI.GetBufferLookup<DamageThisFrame>(),
            DestroyEntityLookup = SystemAPI.GetComponentLookup<DestroyEntityFlag>()
        };

        var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = attackJob.Schedule(simulationSingleton, state.Dependency);
    }
}

public struct PlasmaBlastAttackJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<PlasmaBlastData> PlasmaBlastLookup;
    [ReadOnly] public ComponentLookup<EnemyTag> EnemyLookup;
    public BufferLookup<DamageThisFrame> DamageBufferLookup;
    public ComponentLookup<DestroyEntityFlag> DestroyEntityLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity plasmaBlastEntity;
        Entity enemyEntity;

        if (PlasmaBlastLookup.HasComponent(triggerEvent.EntityA) && EnemyLookup.HasComponent(triggerEvent.EntityB))
        {
            plasmaBlastEntity = triggerEvent.EntityA;
            enemyEntity = triggerEvent.EntityB;
        }
        else if (PlasmaBlastLookup.HasComponent(triggerEvent.EntityB) && EnemyLookup.HasComponent(triggerEvent.EntityA))
        {
            plasmaBlastEntity = triggerEvent.EntityB;
            enemyEntity = triggerEvent.EntityA;
        }
        else
        {
            return;
        }

        var attackDamage = PlasmaBlastLookup[plasmaBlastEntity].AttackDamage;
        var enemyDamageBuffer = DamageBufferLookup[enemyEntity];
        enemyDamageBuffer.Add(new DamageThisFrame { Value = attackDamage });

        DestroyEntityLookup.SetComponentEnabled(plasmaBlastEntity, true);
    }
}
