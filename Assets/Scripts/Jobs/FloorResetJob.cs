using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using LevelDown.Components;
using LevelDown.Components.Singletons;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Shrinks and resets floor tiles that have been "destroyed".
    /// </summary>
    [BurstCompile, WithAll(typeof(Shrinking), typeof(Initialized))]
    public partial struct FloorResetJob : IJobEntity
    {
        public double Time;
        public FloorPhysicsBlobs PhysicsBlobs;
        [NativeDisableParallelForRestriction]
        public ComponentLookup<Shrinking> ShrinkingLookup;
        [NativeDisableParallelForRestriction]
        public ComponentLookup<Initialized> InitializedLookup;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute([ChunkIndexInQuery] int key, Entity entity, FloorBehaviourAspect behaviour)
        {
            var shrink = ShrinkingLookup[entity];

            var elapsed = (float)(Time - shrink.StartTime);
            var finished = elapsed >= shrink.Duration; // Is the "Animation" finished?

            if (finished)
            {
                Ecb.SetEnabled(key, entity, false);
                // Validates and invalidates proper queries
                ShrinkingLookup.SetComponentEnabled(entity, false);
                InitializedLookup.SetComponentEnabled(entity, false);
                behaviour.Reset(PhysicsBlobs);
            }
            else
            {
                behaviour.Scale = math.lerp(behaviour.Tall ? behaviour.TallScale : 1, 0, elapsed / shrink.Duration);
            }
        }
    }
}