using Unity.Entities;
using Unity.Burst;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Updates the player's contols.
    /// </summary>
    [BurstCompile]
    public partial struct PlayerControlJob : IJobEntity
    {
        public void Execute(PlayerControlAspect controls)
        {
            controls.UpdateVelocity();
        }
    }
}
