using Unity.Entities;
using Unity.Rendering;

namespace LevelDown.Components
{
    [MaterialProperty("_Glow")]
    public struct GlowBrightness : IComponentData
    {
        public float Value;
    }
}