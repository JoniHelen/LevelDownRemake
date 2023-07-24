using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using LevelDown.Components;
using Unity.Collections;

namespace LevelDown.Jobs
{
    [BurstCompile]
    public partial struct ColorFlashJob : IJobParallelFor
    {
        [ReadOnly] public NativeList<Entity> Entities;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<ColorFlash> FlashLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<RandomValue> RandomLookup;

        public double Time;

        public void Execute(int index)
        {
            var entity = Entities[index];

            var flash = FlashLookup.GetRefRW(entity);

            FlashLookup.SetComponentEnabled(entity, true);
            flash.ValueRW.StartTime = Time;
            flash.ValueRW.FlashBrightness = 15f;
            flash.ValueRW.FlashColor = UnityEngine.Color.HSVToRGB(RandomLookup.GetRefRW(entity).ValueRW.Value.NextFloat(), 1, 1);
        }
    }
}