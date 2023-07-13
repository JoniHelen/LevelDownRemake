using Unity.Entities;
using Unity.Mathematics;
using LevelDown.Input;

namespace LevelDown.Components.Singletons
{
    /// <summary>
    /// Stores the player's input data.
    /// </summary>
    public struct PlayerInputData : IComponentData
    {
        public float InputLength;
        public float MovementSpeed;
        public float2 MovementDirection;
        public float2 AimDirection;
        public InputButton FireButton;
    }
}