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
        var t = timeSinceStart / aspect.Duration;
        var finished = timeSinceStart >= aspect.Duration;
        var baseGlow = aspect.Tall ? 0f : 1.1f;

        aspect.Color = finished ? aspect.BaseColor : Color.Lerp(aspect.FlashColor, aspect.BaseColor, t);
        aspect.Brightness = finished ? baseGlow : math.lerp(15f, baseGlow, t);
        aspect.Finished = finished;
    }
}