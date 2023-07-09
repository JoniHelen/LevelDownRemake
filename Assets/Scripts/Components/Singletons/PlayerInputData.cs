using Unity.Entities;
using Unity.Mathematics;

namespace LevelDown.Components.Singletons
{
    /// <summary>
    /// Stores the player's input data.
    /// </summary>
    public struct PlayerInputData : IComponentData, IEnableableComponent
    {
        public float InputLength;
        public float MovementSpeed;
        public float2 MovementDirection;
    }
}