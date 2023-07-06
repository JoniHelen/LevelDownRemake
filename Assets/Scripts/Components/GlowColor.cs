using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

namespace LevelDown.Components
{
    [MaterialProperty("_Color")]
    public struct GlowColor : IComponentData
    {
        public Color Color;
    }
}