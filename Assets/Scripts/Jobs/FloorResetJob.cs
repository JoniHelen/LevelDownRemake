using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using LevelDown.Components;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Shrinks and resets floor tiles that have been "destroyed".
    /// </summary>
    [BurstCompile]
    public partial struct FloorResetJob : IJobEntity
    {
        public double Time;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute([ChunkIndexInQuery] int key, Entity entity, FloorBehaviourAspect behaviour,
            EnabledRefRW<Initialized> initialized, ShrinkingAspect shrink)
        {
            var elapsed = (float)(Time - shrink.StartTime);
            var finished = elapsed >= shrink.Duration; // Is the "Animation" finished?

            if (finished)
            {
                Ecb.SetEnabled(key, entity, false);
                // Validates and invalidates proper queries
                initialized.ValueRW = shrink.Enabled = false;
                behaviour.Reset();
            }
            else
            {
                behaviour.Scale = math.lerp(behaviour.Tall ? behaviour.TallScale : 1, 0, elapsed / shrink.Duration);
            }
        }
    }
}