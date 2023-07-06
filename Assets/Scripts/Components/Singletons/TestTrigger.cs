using Unity.Entities;

namespace LevelDown.Components.Singletons
{
    public struct TestTrigger : IComponentData
    {
        public double Interval;
        public double GenerateTime;
        public double DestroyTime;
    }
}