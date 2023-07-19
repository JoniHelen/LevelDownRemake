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
        [ReadOnly] public NativeArray<Entity> Hits;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<RandomValue> RandomValues;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<PhysicsMassOverride> MassOverrides;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<ColorFlash> Flashes;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<Shrinking> Shrinks;

        public void Execute(int index)
        {
            var entity = Hits[index];

            // Find if the entity has already been "destroyed"
            unsafe
            {
                var data = Entities.ListData;
                for (var i = 0; i < data->Length; i++)
                    if (data->ElementAt(i) == entity) return;
            }

            // Flash a color and start "destruction"
            Entities.AddNoResize(entity);
            MassOverrides.GetRefRW(entity).ValueRW.IsKinematic = 0;

            var flash = Flashes.GetRefRW(entity);
            var shrink = Shrinks.GetRefRW(entity);

            flash.ValueRW.StartTime = shrink.ValueRW.StartTime = Time;
            Flashes.SetComponentEnabled(entity, true);
            Shrinks.SetComponentEnabled(entity, true);
            flash.ValueRW.FlashBrightness = 15f;
            flash.ValueRW.FlashColor = UnityEngine.Color.HSVToRGB(RandomValues.GetRefRW(entity).ValueRW.Value.NextFloat(), 1, 1);
        }
    }
}