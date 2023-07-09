using Unity.Entities;
using Unity.Burst;
using LevelDown.Jobs;

namespace LevelDown.Systems
{
    [UpdateAfter(typeof(PlayerInputSystem))]
    public partial struct PlayerControlSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new PlayerControlJob().Schedule();
        }
    }
}