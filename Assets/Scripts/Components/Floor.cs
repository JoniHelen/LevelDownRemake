using Unity.Entities;

namespace LevelDown.Components
{
    /// <summary>
    /// Stores information about the floor tile.
    /// </summary>
    public struct Floor : IComponentData
    {
        public bool Tall;
    }
}