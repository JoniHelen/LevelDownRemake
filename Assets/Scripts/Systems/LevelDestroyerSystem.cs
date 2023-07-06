using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

/// <summary>
/// This system is responsible for destroying completed levels
/// </summary>
public partial struct LevelDestroyerSystem : ISystem, ISystemStartStop
{
    private NativeList<Entity> _entities;

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DestroyLevelTriggerTag>();
        _entities = new(576, Allocator.Persistent);
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnStartRunning(ref SystemState state)
    {
        SystemAPI.GetComponentRW<LevelDestroyerData>(SystemAPI.GetSingletonEntity<LevelDestroyerData>())
            .ValueRW.StartTime = SystemAPI.Time.ElapsedTime;

        _entities.Clear();
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnStopRunning(ref SystemState state)
    {
        
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnDestroy(ref SystemState state)
    {
        _entities.Dispose();
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state)
    {
        var destroyerEntity = SystemAPI.GetSingletonEntity<LevelDestroyerData>();
        var destroyerData = SystemAPI.GetComponentRW<LevelDestroyerData>(destroyerEntity);

        var timeSinceStart = (float)(SystemAPI.Time.ElapsedTime - destroyerData.ValueRO.StartTime);

        if (timeSinceStart < destroyerData.ValueRO.Duration)
        {
            NativeArray<DistanceHit> distanceHits = new(576, Allocator.TempJob);

            state.Dependency = new OverlapSphereJob {
                TimeSinceStart = timeSinceStart,
                CollisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
                Hits = distanceHits,
                DestroyerData = destroyerData.ValueRO
            }.Schedule(state.Dependency);

            state.Dependency = new LevelDestroyerJob {
                Time = SystemAPI.Time.ElapsedTime,
                Hits = distanceHits,
                Entities = _entities.AsParallelWriter()
            }.ScheduleParallel(state.Dependency);
        }
        else
        {
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).RemoveComponent<DestroyLevelTriggerTag>(
                SystemAPI.GetSingletonEntity<TriggerTagSingleton>());
        }
    }
}
