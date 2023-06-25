using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct GlowUpdateJob : IJobEntity
{
    public double Time;
    public double Duration;
    public Color BaseColor;

    [BurstCompile]
    public void Execute(ColorFlashAspect aspect)
    {
        if (aspect.Finished) return;

        double timeSinceStart = Time - aspect.StartTime;

        if (timeSinceStart < Duration)
        {
            aspect.Color = Color.Lerp(aspect.FlashColor, BaseColor, (float)(timeSinceStart / Duration));
            aspect.Brightness = math.lerp(20f, 2f, (float)(timeSinceStart / Duration));
        }
        else
        {
            aspect.Color = BaseColor;
            aspect.Brightness = 2f;
            aspect.Finished = true;
        }
    }
}