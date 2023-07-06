using Unity.Entities;
using Unity.Mathematics;

namespace LevelDown.Components.Singletons
{
    public struct PlayerInput : IComponentData, IEnableableComponent
    {
        public float InputLength;
        public float MovementSpeed;
        public float2 MovementDirection;
    }
}