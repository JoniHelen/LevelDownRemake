using Unity.Entities;
using Unity.Burst;
using LevelDown.Components.Triggers;
using LevelDown.Components.Singletons;

namespace LevelDown.Jobs.Triggers
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public partial struct LevelGenerationTriggerJob : IJobEntity
    {
        public Entity TriggerSingleton;
        public EntityCommandBuffer Ecb;

        public void Execute(ref TestTrigger trigger)
        {
            if (trigger.GenerateTime >= trigger.Interval)
            {
                trigger.GenerateTime = 0;
                Ecb.AddComponent<GenerateLevelTriggerTag>(TriggerSingleton);
            }
        }
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public partial struct LevelDestructionTriggerJob : IJobEntity
    {
        public Entity TriggerSingleton;
        public EntityCommandBuffer Ecb;

        public void Execute(ref TestTrigger trigger)
        {
            if (trigger.DestroyTime >= trigger.Interval)
            {
                trigger.DestroyTime = 0;
                Ecb.AddComponent<DestroyLevelTriggerTag>(TriggerSingleton);
            }
        }
    }
}