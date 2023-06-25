using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Color")]
public struct GlowColorComponent : IComponentData
{
    public Color Color;
}
