using Unity.Entities;
using Unity.Burst;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public partial struct PlayerControlJob : IJobEntity
    {
        public void Execute(PlayerControlAspect controls)
        {
            controls.UpdateVelocity();
        }
    }
}
