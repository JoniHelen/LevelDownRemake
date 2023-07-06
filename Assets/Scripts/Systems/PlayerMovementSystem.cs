using Unity.Entities;
using Unity.Burst;
using LevelDown.Components.Aspects;

namespace LevelDown.Systems
{
    [UpdateAfter(typeof(PlayerInputSystem))]
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var movement in SystemAPI.Query<PlayerMovementAspect>())
                movement.UpdateVelocity();
        }
    }
}