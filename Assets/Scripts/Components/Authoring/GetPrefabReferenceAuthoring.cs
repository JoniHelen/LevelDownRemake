using System.Collections;
using System.Collections.Generic;
using Unity.Entities.Serialization;
using Unity.Entities;
using UnityEngine;

public class GetPrefabAuthoring : MonoBehaviour
{
    public GameObject Prefab;
}

public class GetPrefabBaker : Baker<GetPrefabAuthoring>
{
    public override void Bake(GetPrefabAuthoring authoring)
    {
        var entityPrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic);
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new EntityPrefabComponent() { Value = entityPrefab });
    }
}
