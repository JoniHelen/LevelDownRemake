using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

namespace LevelDown.Components
{
    /// <summary>
    /// A material property override for glow color.
    /// </summary>
    [MaterialProperty("_Color")]
    public struct GlowColor : IComponentData
    {
        public Color Color;
    }
}