using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public readonly partial struct ColorFlashAspect : IAspect
{
    public readonly Entity Self;

    private readonly RefRW<GlowColorComponent> _Color;
    private readonly RefRW<GlowBrightnessComponent> _Brightness;
    private readonly RefRW<ColorFlashComponent> _Flash;

    public Color Color
    {
        get => _Color.ValueRO.Color;
        set => _Color.ValueRW.Color = value;
    }

    public Color FlashColor
    {
        get => _Flash.ValueRO.FlashColor;
        set => _Flash.ValueRW.FlashColor = value;
    }

    public float Brightness
    {
        get => _Brightness.ValueRO.Value;
        set => _Brightness.ValueRW.Value = value;
    }

    public double StartTime
    {
        get => _Flash.ValueRO.StartTime;
        set => _Flash.ValueRW.StartTime = value;
    }

    public bool Finished
    {
        get => _Flash.ValueRO.Finished;
        set => _Flash.ValueRW.Finished = value;
    }
}
