using UnityEngine;
using Unity.Entities;

public struct ColorFlash : IComponentData
{
    public Color FlashColor;
    public Color BaseColor;
    public float Duration;
    public double StartTime;
    public bool Finished;
}