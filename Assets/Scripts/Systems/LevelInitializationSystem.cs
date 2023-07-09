using Unity.Entities;
using Unity.Burst;
using LevelDown.Jobs;
using LevelDown.Components;

namespace LevelDown.Systems
{
    [RequireMatchingQueriesForUpdate]
    public partial struct LevelInitializationSystem : ISystem
    {
        private ComponentLookup<Initialized> _initializedLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _initializedLookup = state.GetComponentLookup<Initialized>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _initializedLookup.Update(ref state);

            // Stop falling
            new FloorStopperJob { InitializedLookup = _initializedLookup }.ScheduleParallel();
        }
    }
}