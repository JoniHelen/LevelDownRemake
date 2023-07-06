using Unity.Entities;
using Unity.Mathematics;

namespace LevelDown.Components
{
    public struct RandomValue : IComponentData
    {
        public Random Value;
    }
}