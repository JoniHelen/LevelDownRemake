using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Glow")]
public struct GlowBrightnessComponent : IComponentData
{
    public float Value;
}
