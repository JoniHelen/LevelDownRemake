using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using LevelDown.Components;
using LevelDown.Components.Triggers;
using LevelDown.Jobs;

namespace LevelDown.Systems
{
    /// <summary>
    /// This system is responsible for destroying completed levels
    /// </summary>
    public partial struct LevelDestroyerSystem : ISystem, ISystemStartStop
    {
        private NativeList<Entity> _entities;
        private ComponentLookup<Shrinking> _shrinkingLookup;
        private ComponentLookup<ColorFlash> _flashLookup;

        private double _startTime;
        private float _duration;
        private float _targetRadius;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _duration = 1;
            _targetRadius = 22;

            state.RequireForUpdate<DestroyLevelTriggerTag>();
            _entities = new(600, Allocator.Persistent);

            _shrinkingLookup = state.GetComponentLookup<Shrinking>();
            _flashLookup = state.GetComponentLookup<ColorFlash>();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            _startTime = SystemAPI.Time.ElapsedTime;
            _entities.Clear();
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _entities.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var timeSinceStart = (float)(SystemAPI.Time.ElapsedTime - _startTime);

            if (timeSinceStart < _duration)
            {
                NativeList<DistanceHit> distanceHits = new(Allocator.TempJob);

                // Overlap a sphere to destory the level
                _ = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld.OverlapSphere(
                    0, _targetRadius * (timeSinceStart / _duration), ref distanceHits, CollisionFilter.Default);

                _shrinkingLookup.Update(ref state);
                _flashLookup.Update(ref state);

                JobHandle destroyerHandle = new LevelDestroyerJob {
                    Time = SystemAPI.Time.ElapsedTime,
                    Hits = distanceHits,
                    FlashLookup = _flashLookup,
                    ShrinkingLookup = _shrinkingLookup,
                    Entities = _entities.AsParallelWriter()
                }.ScheduleParallel(state.Dependency);

                state.Dependency = distanceHits.Dispose(destroyerHandle);
            }
            else
            {
                // Finish execution
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged).RemoveComponent<DestroyLevelTriggerTag>(
                    SystemAPI.GetSingletonEntity<TriggerTagSingleton>());
            }
        }
    }
}