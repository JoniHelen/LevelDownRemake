using Unity.Entities;

namespace LevelDown.Components.Singletons
{
    // Generates a level every 4 seconds
    public struct TestTrigger : IComponentData
    {
        public double Interval;
        public double GenerateTime;
        public double DestroyTime;
    }
}