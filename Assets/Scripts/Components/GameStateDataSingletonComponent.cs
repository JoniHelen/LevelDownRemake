using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct GameStateDataSingletonComponent : IComponentData
{
    public bool RequireNewLevel;
}
