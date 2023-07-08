using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using LevelDown.Components;
using LevelDown.Components.Singletons;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    [WithAll(typeof(Shrinking))]
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public partial struct FloorResetJob : IJobEntity
    {
        public double Time;
        public FloorPhysicsBlobs PhysicsBlobs;
        [NativeDisableParallelForRestriction]
        public ComponentLookup<Shrinking> ShrinkingComponents;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute([ChunkIndexInQuery] int key, Entity entity, FloorBehaviourAspect behaviour)
        {
            var shrink = ShrinkingComponents[entity];

            var elapsed = (float)(Time - shrink.StartTime);
            var finished = elapsed >= shrink.Duration;

            if (finished)
            {
                Ecb.SetEnabled(key, entity, false);
                ShrinkingComponents.SetComponentEnabled(entity, false);
                behaviour.Reset(PhysicsBlobs);
            }
            else
            {
                behaviour.Scale = math.lerp(behaviour.Tall ? behaviour.TallScale : 1, 0, elapsed / shrink.Duration);
            }
        }
    }
}