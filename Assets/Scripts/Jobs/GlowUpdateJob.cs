using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using LevelDown.Components;

namespace LevelDown.Jobs
{
    [WithAll(typeof(ColorFlash))]
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public partial struct GlowUpdateJob : IJobEntity
    {
        public double Time;
        [NativeDisableParallelForRestriction]
        public ComponentLookup<ColorFlash> FlashLookup;

        public void Execute(Entity entity, ref GlowBrightness brightness, ref GlowColor color, ref Floor floor)
        {
            var flash = FlashLookup[entity];

            var timeSinceStart = (float)(Time - flash.StartTime);
            var t = timeSinceStart / flash.Duration;
            var finished = timeSinceStart >= flash.Duration;
            var baseGlow = floor.Tall ? 0f : 1.1f;

            color.Color = finished ? flash.BaseColor : Color.Lerp(flash.FlashColor, flash.BaseColor, t);
            brightness.Value = finished ? baseGlow : math.lerp(15f, baseGlow, t);

            if (finished)
                FlashLookup.SetComponentEnabled(entity, false);
        }
    }
}