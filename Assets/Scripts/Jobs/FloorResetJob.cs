using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using LevelDown.Components;
using LevelDown.Components.Singletons;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public partial struct FloorResetJob : IJobEntity
    {
        public double Time;
        public FloorPhysicsBlobs PhysicsBlobs;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute([ChunkIndexInQuery] int key, Entity entity, ref Shrinking shrink, FloorBehaviourAspect behaviour)
        {
            if (shrink.Finished) return;

            var elapsed = (float)(Time - shrink.StartTime);
            var finished = elapsed >= shrink.Duration;

            if (finished)
            {
                Ecb.SetEnabled(key, entity, false);

                shrink.Finished = finished;
                behaviour.Reset(PhysicsBlobs);
            }
            else
            {
                behaviour.Scale = math.lerp(behaviour.Tall ? behaviour.TallScale : 1, 0, elapsed / shrink.Duration);
            }
        }
    }
}