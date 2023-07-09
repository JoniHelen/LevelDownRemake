using Unity.Entities;
using Unity.Burst;
using LevelDown.Jobs;
using LevelDown.Components.Singletons;
using LevelDown.Components;

namespace LevelDown.Systems
{
    /// <summary>
    /// This system updates and resets floor entities when switching levels
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    public partial struct FloorShrinkingSystem : ISystem
    {
        private ComponentLookup<Shrinking> _shrinkingLookup;
        private ComponentLookup<Initialized> _initializedLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _shrinkingLookup = state.GetComponentLookup<Shrinking>();
            _initializedLookup = state.GetComponentLookup<Initialized>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _shrinkingLookup.Update(ref state);
            _initializedLookup.Update(ref state);

            new FloorResetJob
            {
                Time = SystemAPI.Time.ElapsedTime,
                ShrinkingLookup = _shrinkingLookup,
                InitializedLookup = _initializedLookup,
                PhysicsBlobs = SystemAPI.GetSingleton<FloorPhysicsBlobs>(),
                Ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
        }
    }
}