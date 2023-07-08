using Unity.Entities;
using Unity.Mathematics;

namespace LevelDown.Components.Singletons
{
    public struct TempPlayerPos : IComponentData
    {
        public float2 Value;
    }
}