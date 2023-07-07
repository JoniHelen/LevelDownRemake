using Unity.Entities;
using LevelDown.Jobs;

namespace LevelDown.Systems
{
    public partial struct PlayerInputSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            new InputGatheringJob().Schedule();
        }
    }
}