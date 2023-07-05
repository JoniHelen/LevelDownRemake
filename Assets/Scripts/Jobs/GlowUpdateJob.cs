using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct GlowUpdateJob : IJobEntity
{
    public double Time;

    public void Execute(ColorFlashAspect aspect)
    {
        if (aspect.Finished) return;

        var timeSinceStart = (float)(Time - aspect.StartTime);
        var baseGlow = aspect.Tall ? 0f : 1.1f;

        aspect.Color = timeSinceStart < aspect.Duration ? Color.Lerp(aspect.FlashColor, aspect.BaseColor, timeSinceStart / aspect.Duration) : aspect.BaseColor;
        aspect.Brightness = timeSinceStart < aspect.Duration ? math.lerp(15f, baseGlow, timeSinceStart / aspect.Duration) : baseGlow;
        aspect.Finished = timeSinceStart >= aspect.Duration;
    }
}