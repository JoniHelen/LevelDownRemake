using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Collections;
using LevelDown.Components.Triggers;
using LevelDown.Components;
using LevelDown.Jobs;
using EndSimulation =
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    /// <summary>
    /// This system is responsible for destroying completed levels
    /// </summary>
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct LevelDestroyerSystem : ISystem, ISystemStartStop
    {
        private ComponentLookup<PhysicsMassOverride> _massOverrideLookup;
        private ComponentLookup<Shrinking> _shrinkLookup;
        private ComponentLookup<RandomValue> _randomLookup;
        private ComponentLookup<ColorFlash> _flashLookup;

        private NativeParallelHashSet<Entity> _destroyedEntities;

        private double _startTime;
        private float _duration;
        private float _targetRadius;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _duration = 1;
            _targetRadius = 20;

            _massOverrideLookup = state.GetComponentLookup<PhysicsMassOverride>();
            _shrinkLookup = state.GetComponentLookup<Shrinking>();
            _flashLookup = state.GetComponentLookup<ColorFlash>();
            _randomLookup = state.GetComponentLookup<RandomValue>();

            state.RequireForUpdate<DestroyLevelTriggerTag>();
            _destroyedEntities = new(600, Allocator.Persistent);
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            _startTime = SystemAPI.Time.ElapsedTime;
            _destroyedEntities.Clear();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var timeSinceStart = (float)(SystemAPI.Time.ElapsedTime - _startTime);

            if (timeSinceStart < _duration)
            {
                var hitEntities = new NativeList<Entity>(100, Allocator.TempJob);
                var radius = timeSinceStart / _duration * _targetRadius;
                var collector = new FloorCollectorHash {
                    Comparison = _destroyedEntities,
                    Hits = hitEntities,
                    MaxFraction = radius
                };
                SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld
                    .OverlapSphereCustom(new float3(0, 0, -1), radius, ref collector, new CollisionFilter
                    {
                        BelongsTo = 1u << 31,
                        CollidesWith = (1u << 2) | (1u << 3),
                        GroupIndex = 0
                    });

                _massOverrideLookup.Update(ref state);
                _shrinkLookup.Update(ref state);
                _flashLookup.Update(ref state);
                _randomLookup.Update(ref state);

                JobHandle destroyerHandle = new LevelDestroyerJob {
                    Time = SystemAPI.Time.ElapsedTime,
                    Hits = hitEntities.AsArray(),
                    Entities = _destroyedEntities.AsParallelWriter(),
                    MassLookup = _massOverrideLookup,
                    ShrinkLookup = _shrinkLookup,
                    FlashLookup = _flashLookup,
                    RandomLookup = _randomLookup
                }.Schedule(hitEntities.Length, 8, state.Dependency);

                state.Dependency = hitEntities.Dispose(destroyerHandle);
            }
            else
            {
                // Finish execution
                SystemAPI.GetSingleton<EndSimulation>()
                    .CreateCommandBuffer(state.WorldUnmanaged)
                    .RemoveComponent<DestroyLevelTriggerTag>(
                    SystemAPI.GetSingletonEntity<TriggerTagSingleton>());
            }
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _destroyedEntities.Dispose();
        }
    }
}