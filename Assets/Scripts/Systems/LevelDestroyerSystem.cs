using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using LevelDown.Components.Triggers;
using LevelDown.Jobs;
using EndSimulation =
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    /// <summary>
    /// This system is responsible for destroying completed levels
    /// </summary>
    public partial struct LevelDestroyerSystem : ISystem, ISystemStartStop
    {
        private NativeList<Entity> _entities;

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
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            _startTime = SystemAPI.Time.ElapsedTime;
            _entities.Clear();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var timeSinceStart = (float)(SystemAPI.Time.ElapsedTime - _startTime);

            if (timeSinceStart < _duration)
            {
                NativeList<DistanceHit> distanceHits = new(Allocator.TempJob);

                // Overlap a sphere to destory the level
                _ = SystemAPI.GetSingleton<PhysicsWorldSingleton>()
                    .OverlapSphere(0, _targetRadius * (timeSinceStart / _duration), ref distanceHits,
                    new CollisionFilter { BelongsTo = 1u << 31, CollidesWith = 1u << 2 | 1u << 3, GroupIndex = 0 });

                JobHandle destroyerHandle = new LevelDestroyerJob {
                    Time = SystemAPI.Time.ElapsedTime,
                    Hits = distanceHits,
                    Entities = _entities.AsParallelWriter()
                }.ScheduleParallel(state.Dependency);

                state.Dependency = distanceHits.Dispose(destroyerHandle);
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
            _entities.Dispose();
        }
    }
}