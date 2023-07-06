using Unity.Entities;

namespace LevelDown.Components.Singletons
{
    public struct LevelDestroyerData : IComponentData
    {
        public double StartTime; 
        public float Duration; 
        public float TargetRadius; 
        public bool Finished;
    }
}