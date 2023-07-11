using Unity.Entities;
using Unity.Burst;
using LevelDown.Jobs;
using EndSimulation = 
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    /// <summary>
    /// This system updates and resets floor entities when switching levels
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    public partial struct FloorShrinkingSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) => new FloorResetJob {
            Time = SystemAPI.Time.ElapsedTime,
            Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();
    }
}