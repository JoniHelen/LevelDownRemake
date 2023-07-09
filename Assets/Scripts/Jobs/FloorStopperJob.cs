using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using LevelDown.Components;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Stops initialized floor tiles at the proper point in space.
    /// </summary>
    [BurstCompile, WithDisabled(typeof(Initialized), typeof(Shrinking))]
    public partial struct FloorStopperJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        public ComponentLookup<Initialized> InitializedLookup;

        public void Execute(Entity entity, FloorBehaviourAspect behaviour)
        {
            if (behaviour.Position.z <= 0)
            {
                behaviour.Stop();
                behaviour.Position = new(behaviour.Position.xy, behaviour.Tall ? -0.5f : 0);
                InitializedLookup.SetComponentEnabled(entity, true);
            }
        }
    }
}
