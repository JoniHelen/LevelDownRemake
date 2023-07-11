using Unity.Entities;
using Unity.Burst;
using LevelDown.Jobs;

namespace LevelDown.Systems
{
    /// <summary>
    /// This system is responsible for updating the flashing floor tiles.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    public partial struct ColorFlashSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) => new GlowUpdateJob {
            Time = SystemAPI.Time.ElapsedTime
        }.ScheduleParallel();
    }
}