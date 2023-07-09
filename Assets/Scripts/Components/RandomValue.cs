using Unity.Entities;
using Unity.Mathematics;

namespace LevelDown.Components
{
    /// <summary>
    /// Stores a random number generator.
    /// </summary>
    public struct RandomValue : IComponentData
    {
        public Random Value;
    }
}