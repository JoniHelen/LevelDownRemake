using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using LevelDown.Components.Triggers;
using LevelDown.Components.Tags;
using LevelDown.Components;
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
        private ComponentLookup<PhysicsMassOverride> _massOverrideLookup;
        private ComponentLookup<Shrinking> _shrinkLookup;

        private NativeList<Entity> _destroyedEntities;

        private double _startTime;
        private float _duration;
        private float _targetRadius;

        [BurstCompile]
        private struct EntityDifferenceJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Entity> NewEntities;
            [ReadOnly] public NativeArray<Entity> DestroyedEntities;
            public NativeList<Entity>.ParallelWriter Difference;
            public void Execute(int index)
            {
                var entity = NewEntities[index];

                if (DestroyedEntities.Contains(entity)) return;

                Difference.AddNoResize(entity);
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _duration = 1;
            _targetRadius = 40;

            _massOverrideLookup = state.GetComponentLookup<PhysicsMassOverride>();
            _shrinkLookup = state.GetComponentLookup<Shrinking>();

            state.RequireForUpdate<DestroyLevelTriggerTag>();
            _destroyedEntities = new(600, Allocator.Persistent);
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            _startTime = SystemAPI.Time.ElapsedTime;
            var destroyer = SystemAPI.QueryBuilder()
                    .WithAll<LevelDestroyer>()
                    .WithOptions(EntityQueryOptions.IncludeDisabledEntities).Build()
                    .GetSingletonEntity();

            state.EntityManager.SetEnabled(destroyer, true);
            SystemAPI.GetBuffer<EntityBufferData>(destroyer).Clear();
            _destroyedEntities.Clear();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var timeSinceStart = (float)(SystemAPI.Time.ElapsedTime - _startTime);

            if (timeSinceStart < _duration)
            {
                var destroyer = SystemAPI.GetSingletonEntity<LevelDestroyer>();
                SystemAPI.GetComponentRW<LocalTransform>(destroyer).ValueRW.Scale = timeSinceStart / _duration * _targetRadius;

                var hitEntities = new NativeList<Entity>(200, Allocator.TempJob);
                var newEntities = SystemAPI.GetBuffer<EntityBufferData>(destroyer).Reinterpret<Entity>().AsNativeArray();

                JobHandle diff = new EntityDifferenceJob
                {
                    NewEntities = newEntities,
                    DestroyedEntities = _destroyedEntities.AsArray(),
                    Difference = hitEntities.AsParallelWriter()
                }.Schedule(newEntities.Length, 8, default);

                _massOverrideLookup.Update(ref state);
                _shrinkLookup.Update(ref state);

                diff.Complete();

                JobHandle destroyerHandle = new LevelDestroyerJob {
                    Time = SystemAPI.Time.ElapsedTime,
                    Hits = hitEntities,
                    Entities = _destroyedEntities.AsParallelWriter(),
                    MassOverrides = _massOverrideLookup,
                    Shrinks = _shrinkLookup
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

                var destroyer = SystemAPI.GetSingletonEntity<LevelDestroyer>();
                state.EntityManager.SetEnabled(destroyer, false);
                SystemAPI.GetComponentRW<LocalTransform>(destroyer).ValueRW.Scale = 0.0001f;

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