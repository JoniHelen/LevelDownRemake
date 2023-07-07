using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Collections;
using LevelDown.Components.Triggers;
using LevelDown.Components.Singletons;
using LevelDown.Jobs;

namespace LevelDown.Systems
{
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
                NativeList<DistanceHit> distanceHits = new(Allocator.TempJob);

                _ = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld.OverlapSphere(
                    0, destroyerData.ValueRO.TargetRadius * (timeSinceStart / destroyerData.ValueRO.Duration), ref distanceHits, CollisionFilter.Default);

                state.Dependency = new LevelDestroyerJob
                {
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
}