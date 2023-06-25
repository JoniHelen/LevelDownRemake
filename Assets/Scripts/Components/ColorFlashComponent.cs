using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct ColorFlashComponent : IComponentData
{
    public Color FlashColor;
    public double StartTime;
    public bool Finished;
}
