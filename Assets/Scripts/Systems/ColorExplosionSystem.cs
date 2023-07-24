using Unity.Entities;
using Unity.Burst;
using LevelDown.Jobs;
using EndSimulation =
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    [RequireMatchingQueriesForUpdate, UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ColorExplosionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ColorExplosionJob
            {
                Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                Time = SystemAPI.Time.ElapsedTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        
        }
    }
}