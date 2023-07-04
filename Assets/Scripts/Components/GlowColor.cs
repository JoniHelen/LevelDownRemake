using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Color")]
public struct GlowColor : IComponentData
{
    public Color Color;
}