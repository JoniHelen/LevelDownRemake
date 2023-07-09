using Unity.Entities;
using Unity.Burst;
using LevelDown.Components.Triggers;
using LevelDown.Components.Singletons;

namespace LevelDown.Jobs.Triggers
{
    /// <summary>
    /// Triggers level generation if the time is right.
    /// </summary>
    [BurstCompile]
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

    /// <summary>
    /// Destorys and generates a new level at the same time after an inteval.
    /// </summary>
    [BurstCompile]
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
                Ecb.AddComponent<GenerateLevelTriggerTag>(TriggerSingleton);
            }
        }
    }
}