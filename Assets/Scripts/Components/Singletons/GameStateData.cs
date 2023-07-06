using Unity.Entities;

namespace LevelDown.Components.Singletons
{
    public struct GameStateData : IComponentData
    {
        public bool RequireNewLevel;
        public bool RequireLevelDestruction;
        public double DestroyTime;
    }
}