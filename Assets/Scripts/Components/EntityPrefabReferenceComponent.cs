using System.Collections;
using System.Collections.Generic;
using Unity.Entities.Serialization;
using Unity.Entities;
using UnityEngine;

public struct EntityPrefabComponent : IComponentData
{
    public Entity Value;
}