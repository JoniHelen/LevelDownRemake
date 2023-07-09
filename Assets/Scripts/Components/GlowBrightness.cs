using Unity.Entities;
using Unity.Rendering;

namespace LevelDown.Components
{
    /// <summary>
    /// A material property override for glow brightness.
    /// </summary>
    [MaterialProperty("_Glow")]
    public struct GlowBrightness : IComponentData
    {
        public float Value;
    }
}