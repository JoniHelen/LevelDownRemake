using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Physics;
using LevelDown.Components;

namespace LevelDown.Jobs
{
    /// <summary>
    /// "Destroys" the floor tiles that get hit during level destruction.
    /// </summary>
    [BurstCompile]
    public partial struct LevelDestroyerJob : IJobParallelFor
    {
        public double Time;
        public NativeList<Entity>.ParallelWriter Entities;
        [ReadOnly] public NativeList<Entity> Hits;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<PhysicsMassOverride> MassOverrides;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<Shrinking> Shrinks;

        public void Execute(int index)
        {
            var entity = Hits[index];

            // Start "destruction"
            Entities.AddNoResize(entity);
            MassOverrides.GetRefRW(entity).ValueRW.IsKinematic = 0;

            var shrink = Shrinks.GetRefRW(entity);
            shrink.ValueRW.StartTime = Time;
            Shrinks.SetComponentEnabled(entity, true);
        }
    }
}