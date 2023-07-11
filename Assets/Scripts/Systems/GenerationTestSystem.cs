using Unity.Entities;
using Unity.Burst;
using LevelDown.Components.Triggers;
using LevelDown.Components.Singletons;
using LevelDown.Jobs.Triggers;
using EndSimulation =
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    public partial struct GenerationTestSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var trigger = SystemAPI.GetSingletonEntity<TriggerTagSingleton>();

            var test = SystemAPI.GetSingletonRW<TestTrigger>();

            test.ValueRW.DestroyTime += SystemAPI.Time.DeltaTime;

            new LevelDestructionTriggerJob
            {
                Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged),
                TriggerSingleton = trigger
            }.Schedule();
        }
    }
}