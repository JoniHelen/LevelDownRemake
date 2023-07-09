using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Physics;
using Unity.Mathematics;
using LevelDown.Jobs;
using LevelDown.Components;
using LevelDown.Components.Singletons;
using LevelDown.Components.Triggers;

namespace LevelDown.Systems
{
    /// <summary>
    /// This system is responsible for initializing all entities used during runtime.
    /// Also frees memory used by allocated component data.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct EntityInitializationSystem : ISystem, ISystemStartStop
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntityPrefab>();

            #region SINGLETON_INIT
            // Trigger Singleton
            state.EntityManager.CreateSingleton<TriggerTagSingleton>();

            // Colliders
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
                CollidesWith = uint.MaxValue ^ (1u << 3), // Everything but TallFloor
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

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            // Instantiate floor entities
            var prefab = SystemAPI.GetSingleton<EntityPrefab>().Value;
            state.EntityManager.Instantiate(prefab, 1300, Allocator.Temp);

            // Get the small physics shape blob asset
            SystemAPI.GetSingletonRW<FloorPhysicsBlobs>()
                .ValueRW.Small = SystemAPI.GetComponent<PhysicsCollider>(prefab).Value;

            // Seed the flash randoms
            new RandomSeedJob().ScheduleParallel();

            /*state.EntityManager.AddComponent<GenerateLevelTriggerTag>(
                SystemAPI.GetSingletonEntity<TriggerTagSingleton>());*/

            // No update is required
            state.Enabled = false;
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            // Must implement from interface
        }
    }

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [CreateAfter(typeof(FixedStepSimulationSystemGroup))]
    public partial struct FixedStepOverrideSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            // TODO: make this more manageable for lower end systems by scanning display refresh rates
            // Possibly tie this to settings regarding FPS limits
            state.World.GetExistingSystemManaged<FixedStepSimulationSystemGroup>().Timestep = 1f / 144;
            state.Enabled = false;
        }
    }
}