using Unity.Entities;

namespace LevelDown.Components
{
    public struct Shrinking : IComponentData, IEnableableComponent
    {
        public double StartTime; 
        public float Duration;
    }
}