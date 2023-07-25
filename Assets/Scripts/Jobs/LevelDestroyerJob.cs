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
        public NativeParallelHashSet<Entity>.ParallelWriter Entities;
        [ReadOnly] public NativeArray<Entity> Hits;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<PhysicsMassOverride> MassLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<Shrinking> ShrinkLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<ColorFlash> FlashLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<RandomValue> RandomLookup;

        public void Execute(int index)
        {
            var entity = Hits[index];

            // Start "destruction"
            Entities.Add(entity);
            MassLookup.GetRefRW(entity).ValueRW.IsKinematic = 0;

            var shrink = ShrinkLookup.GetRefRW(entity);
            var flash = FlashLookup.GetRefRW(entity);
            shrink.ValueRW.StartTime = flash.ValueRW.StartTime = Time;
            flash.ValueRW.FlashBrightness = 15f;
            flash.ValueRW.FlashColor = UnityEngine.Color.HSVToRGB(RandomLookup.GetRefRW(entity).ValueRW.Value.NextFloat(), 1, 1);
            ShrinkLookup.SetComponentEnabled(entity, true);
            FlashLookup.SetComponentEnabled(entity, true);
        }
    }
}