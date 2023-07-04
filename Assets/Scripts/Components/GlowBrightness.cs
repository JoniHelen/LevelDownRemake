using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Glow")]
public struct GlowBrightness : IComponentData
{
    public float Value;
}