using Unity.Entities;
using Unity.Burst;
using LevelDown.Jobs;

namespace LevelDown.Systems
{
    [UpdateAfter(typeof(PlayerInputSystem))]
    public partial struct PlayerControlSystem : ISystem
    {
        [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            new PlayerControlJob().Schedule();
        }
    }
}