using Unity.Entities;

namespace LevelDown.Components
{
    /// <summary>
    /// Stores an entity prefab as data.
    /// </summary>
    public struct EntityPrefab : IComponentData
    {
        public Entity Value;
    }
}