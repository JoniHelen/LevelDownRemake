using Unity.Entities;

namespace LevelDown.Components
{
    public struct EntityBuffer : IBufferElementData
    {
        public Entity entity;
    }
}