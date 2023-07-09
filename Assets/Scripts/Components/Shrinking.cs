using Unity.Entities;

namespace LevelDown.Components
{
    /// <summary>
    /// Stores data about floor tiles that are being destroyed.
    /// </summary>
    public struct Shrinking : IComponentData, IEnableableComponent
    {
        public double StartTime; 
        public float Duration;
    }
}