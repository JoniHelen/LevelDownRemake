using Unity.Entities;
using Unity.Burst;
using LevelDown.Jobs;

namespace LevelDown.Systems
{
    [RequireMatchingQueriesForUpdate]
    public partial struct LevelInitializationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) => new FloorStopperJob().ScheduleParallel();
    }
}