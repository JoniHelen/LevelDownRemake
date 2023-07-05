using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

/// <summary>
/// This system is responsible for destroying completed levels
/// </summary>
public partial struct LevelDestroyerSystem : ISystem, ISystemStartStop
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnCreate(ref SystemState state) => state.RequireForUpdate<DestroyLevelTriggerTag>();

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnStartRunning(ref SystemState state)
    {
        var destroyerEntity = SystemAPI.GetSingletonEntity<LevelDestroyerData>();
        var destroyerData = SystemAPI.GetComponentRW<LevelDestroyerData>(destroyerEntity);

        destroyerData.ValueRW.StartTime = SystemAPI.Time.ElapsedTime;
        SystemAPI.GetBuffer<EntityBuffer>(destroyerEntity).Clear();
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnStopRunning(ref SystemState state)
    {
        
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state)
    {
        var destroyerEntity = SystemAPI.GetSingletonEntity<LevelDestroyerData>();
        var destroyerData = SystemAPI.GetComponentRW<LevelDestroyerData>(destroyerEntity);

        var timeSinceStart = (float)(SystemAPI.Time.ElapsedTime - destroyerData.ValueRO.StartTime);

        if (timeSinceStart < destroyerData.ValueRO.Duration)
        {
            NativeList<DistanceHit> distanceHits = new(state.WorldUpdateAllocator);

            _ = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld
                .OverlapSphere(0, math.lerp(0, destroyerData.ValueRO.TargetRadius, timeSinceStart / destroyerData.ValueRO.Duration), ref distanceHits, CollisionFilter.Default);

            new LevelDestroyerJob {
                Time = SystemAPI.Time.ElapsedTime,
                Hits = distanceHits,
                DestroyerSingleton = destroyerEntity,
                Buffer = SystemAPI.GetBuffer<EntityBuffer>(destroyerEntity),
                Ecb = SystemAPI.GetSingletonRW<EndSimulationEntityCommandBufferSystem.Singleton>()
                .ValueRW.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
        }
        else
        {
            SystemAPI.GetSingletonRW<EndSimulationEntityCommandBufferSystem.Singleton>()
                .ValueRW.CreateCommandBuffer(state.WorldUnmanaged).RemoveComponent<DestroyLevelTriggerTag>(
                SystemAPI.GetSingletonEntity<TriggerTagSingleton>());
        }
    }
}
