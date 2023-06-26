using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct EntityDynamicBufferComponent : IBufferElementData
{
    public Entity entity;
}
