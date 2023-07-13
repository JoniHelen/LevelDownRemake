using Unity.Entities;
using Unity.Burst;
using LevelDown.Components;
using LevelDown.Components.Tags;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Stops initialized floor tiles at the proper point in space.
    /// </summary>
    [BurstCompile, WithDisabled(typeof(Initialized), typeof(Shrinking))]
    public partial struct FloorStopperJob : IJobEntity
    {
        public void Execute(FloorBehaviourAspect behaviour, EnabledRefRW<Initialized> initialized)
        {
            if (behaviour.Position.z <= 0)
            {
                behaviour.Stop();
                behaviour.Position = new(behaviour.Position.xy, behaviour.Tall ? -0.5f : 0);
                initialized.ValueRW = true;
            }
        }
    }
}