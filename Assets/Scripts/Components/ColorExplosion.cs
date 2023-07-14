using Unity.Entities;

namespace LevelDown.Components
{
    public struct ColorExplosion : IComponentData, IEnableableComponent
    {
        public double StartTime;
        public float TargetSize;
        public float Duration;
    }
}