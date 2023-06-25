using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct FloorDataSingletonComponent : IComponentData
{
    public Color BaseColor;
    public double Duration;
}
