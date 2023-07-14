using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics;
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
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<Floor, Prefab>()
                .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities).Build());

            #region SINGLETON_INIT
            // Trigger Singleton
            state.EntityManager.CreateSingleton<TriggerTagSingleton>();

            // Entity queue
            state.EntityManager.CreateSingleton(new ProjectileQueue { Projectiles = new NativeQueue<Input.ProjectileDescriptor>(Allocator.Persistent) });

            // Test Singleton
            state.EntityManager.CreateSingleton(new TestTrigger { Interval = 4, GenerateTime = 2 });
            #endregion
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            // Instantiate floor entities
            var prefab = SystemAPI.QueryBuilder().WithAll<Floor, Prefab>()
                .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities).Build()
                .GetSingletonEntity();

            // Collider blobs
            var colliderTall = BoxCollider.Create(new BoxGeometry
            {
                BevelRadius = 0.05f,
                Center = 0,
                Orientation = quaternion.identity,
                Size = new float3(1, 1, 2)
            });

            colliderTall.Value.SetCollisionFilter(new CollisionFilter
            {
                BelongsTo = 1u << 3, // TallFloor
                CollidesWith = 1u << 31 | 1u << 4,
                GroupIndex = 0
            });

            state.EntityManager.AddSharedComponent(prefab, new FloorPhysicsBlobs { 
                Tall = colliderTall, 
                Small = SystemAPI.GetComponent<PhysicsCollider>(prefab).Value 
            });

            state.EntityManager.Instantiate(prefab, 1300, Allocator.Temp);

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