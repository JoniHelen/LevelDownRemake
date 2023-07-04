using Unity.Entities;
using Unity.Burst;

[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct LevelGenerationTriggerJob : IJobEntity
{
    public Entity triggerSingleton;
    public EntityCommandBuffer ecb;

    public void Execute(ref TestTrigger trigger)
    {
        if (trigger.GenerateTime >= trigger.Interval)
        {
            trigger.GenerateTime = 0;
            ecb.AddComponent<GenerateLevelTriggerTag>(triggerSingleton);
        }
    }
}

[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct LevelDestructionTriggerJob : IJobEntity
{
    public Entity triggerSingleton;
    public EntityCommandBuffer ecb;

    public void Execute(ref TestTrigger trigger)
    {
        if (trigger.DestroyTime >= trigger.Interval)
        {
            trigger.DestroyTime = 0;
            ecb.AddComponent<DestroyLevelTriggerTag>(triggerSingleton);
        }
    }
}