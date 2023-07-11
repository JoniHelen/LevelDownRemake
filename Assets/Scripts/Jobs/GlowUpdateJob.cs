using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Manages the glow color and brightness of all floor tiles.
    /// </summary>
    [BurstCompile]
    public partial struct GlowUpdateJob : IJobEntity
    {
        public double Time;

        public void Execute(ColorControlAspect control)
        {
            var timeSinceStart = (float)(Time - control.StartTime);
            var t = timeSinceStart / control.Duration;
            var finished = timeSinceStart >= control.Duration;
            var baseGlow = control.Tall ? 0f : 1.1f;

            // Lerp values based on time elapsed
            control.Color = finished ? control.BaseColor : Color.Lerp(control.FlashColor, control.BaseColor, t);
            control.Brightness = finished ? baseGlow : math.lerp(15f, baseGlow, t);

            if (finished)
                control.Enabled = false;
        }
    }
}