using Unity.Entities;

namespace LevelDown.Components
{
    /// <summary>
    /// Can be used as a dynamic buffer to store entities.
    /// </summary>
    public struct EntityBuffer : IBufferElementData
    {
        public Entity entity;
    }
}