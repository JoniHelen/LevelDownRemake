using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Physics;
using Unity.Mathematics;

/// <summary>
/// This system is responsible for initializing all entities used during runtime
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
        Entity destroyerData = state.EntityManager.CreateSingleton<LevelDestroyerData>();
        state.EntityManager.AddBuffer<EntityBuffer>(destroyerData);
        state.EntityManager.SetComponentData(destroyerData, new LevelDestroyerData {
            Duration = 1,
            TargetRadius = 22
        });

        // Trigger Singleton
        state.EntityManager.CreateSingleton<TriggerTagSingleton>();

        var collider = BoxCollider.Create(new BoxGeometry
        {
            BevelRadius = 0.05f,
            Center = 0,
            Orientation = quaternion.identity,
            Size = new float3(1, 1, 2)
        });

        collider.Value.SetCollisionFilter(new CollisionFilter
        {
            BelongsTo = 1 << 3, // TallFloor
            CollidesWith = uint.MaxValue ^ (1 << 3), // Everything but Floor
            GroupIndex = 0
        });

        state.EntityManager.SetComponentData(
            state.EntityManager.CreateSingleton<FloorPhysicsBlobs>(),
            new FloorPhysicsBlobs { Tall = collider });

        // Test Singleton
        Entity test = state.EntityManager.CreateSingleton<TestTrigger>();
        state.EntityManager.SetComponentData(test, new TestTrigger { Interval = 4, GenerateTime = 2 });
        #endregion
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnStartRunning(ref SystemState state)
    {
        var prefab = SystemAPI.GetSingleton<EntityPrefab>().Value;

        state.EntityManager.Instantiate(prefab, 1300, Allocator.Temp);

        SystemAPI.GetSingletonRW<FloorPhysicsBlobs>()
            .ValueRW.Small = SystemAPI.GetComponent<PhysicsCollider>(prefab).Value;

        new RandomSeedJob().ScheduleParallel();
        new SetSimulateJob { 
            BoolToSet = false, 
            Ecb = SystemAPI.GetSingletonRW<EndInitializationEntityCommandBufferSystem.Singleton>()
            .ValueRW.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();

        /*state.EntityManager.AddComponent<GenerateLevelTriggerTag>(
            SystemAPI.GetSingletonEntity<TriggerTagSingleton>());*/
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
