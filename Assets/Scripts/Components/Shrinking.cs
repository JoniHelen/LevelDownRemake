using Unity.Entities;

namespace LevelDown.Components
{
    public struct Shrinking : IComponentData
    {
        public double StartTime; 
        public float Duration; 
        public bool Finished;
    }
}