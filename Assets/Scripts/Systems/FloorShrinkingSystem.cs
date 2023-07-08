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

        [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
        public void OnCreate(ref SystemState state)
        {
            _shrinkingLookup = state.GetComponentLookup<Shrinking>();
        }

        [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            _shrinkingLookup.Update(ref state);

            new FloorResetJob
            {
                Time = SystemAPI.Time.ElapsedTime,
                ShrinkingComponents = _shrinkingLookup,
                PhysicsBlobs = SystemAPI.GetSingleton<FloorPhysicsBlobs>(),
                Ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
        }
    }
}