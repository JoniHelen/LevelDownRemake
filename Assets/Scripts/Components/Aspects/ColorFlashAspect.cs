using UnityEngine;
using Unity.Entities;

public readonly partial struct ColorFlashAspect : IAspect
{
    public readonly Entity Self;

    private readonly RefRW<GlowColor> _Color;
    private readonly RefRW<GlowBrightness> _Brightness;
    private readonly RefRW<ColorFlash> _Flash;
    private readonly RefRW<Floor> _Floor;

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
    public Color BaseColor
    {
        get => _Flash.ValueRO.BaseColor;
        set => _Flash.ValueRW.BaseColor = value;
    }

    public float Duration
    {
        get => _Flash.ValueRO.Duration;
        set => _Flash.ValueRW.Duration = value;
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

    public bool Tall
    {
        get => _Floor.ValueRO.Tall;
        set => _Floor.ValueRW.Tall = value;
    }
}