using Unity.Entities;
using Unity.Burst;

public partial struct GenerationTestSystem : ISystem
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state)
    {
        Entity trigger = SystemAPI.GetSingletonEntity<TriggerTagSingleton>();

        var test = SystemAPI.GetSingletonRW<TestTrigger>();

        test.ValueRW.GenerateTime += SystemAPI.Time.DeltaTime;
        test.ValueRW.DestroyTime += SystemAPI.Time.DeltaTime;

        new LevelGenerationTriggerJob {
            ecb = SystemAPI.GetSingletonRW<EndSimulationEntityCommandBufferSystem.Singleton>()
            .ValueRW.CreateCommandBuffer(state.WorldUnmanaged),
            triggerSingleton = trigger
        }.Schedule();

        new LevelDestructionTriggerJob {
            ecb = SystemAPI.GetSingletonRW<EndSimulationEntityCommandBufferSystem.Singleton>()
            .ValueRW.CreateCommandBuffer(state.WorldUnmanaged),
            triggerSingleton = trigger
        }.Schedule();
    }
}
