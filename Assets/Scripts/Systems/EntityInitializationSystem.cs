using Unity.Entities;
using Unity.Burst;
using Unity.Collections;

/// <summary>
/// This system is responsible for initializing all entities used during runtime.
/// </summary>
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct EntityInitializationSystem : ISystem, ISystemStartStop
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntityPrefab>();

        #region SINGLETON_INIT
        // Destroyer data
        Entity destroyer = state.EntityManager.CreateSingleton<LevelDestroyerData>();
        state.EntityManager.AddBuffer<EntityBuffer>(destroyer);
        state.EntityManager.SetComponentData(destroyer, new LevelDestroyerData {
            Duration = 1,
            TargetRadius = 22
        });

        // Trigger Singleton
        state.EntityManager.CreateSingleton<TriggerTagSingleton>();


        // Test Singleton
        Entity test = state.EntityManager.CreateSingleton<TestTrigger>();
        state.EntityManager.SetComponentData(test, new TestTrigger { Interval = 4, GenerateTime = 2 });
        #endregion
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnStartRunning(ref SystemState state)
    {
        state.EntityManager.Instantiate(SystemAPI.GetSingleton<EntityPrefab>().Value, 1300, Allocator.Temp);

        new PhysicsColliderGenerationJob().ScheduleParallel();
        new RandomSeedJob().ScheduleParallel();
        new SetSimulateJob { 
            boolToSet = false, 
            ecb = SystemAPI.GetSingletonRW<EndInitializationEntityCommandBufferSystem.Singleton>()
            .ValueRW.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state)
    {

    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnStopRunning(ref SystemState state)
    {
        
    }
}
