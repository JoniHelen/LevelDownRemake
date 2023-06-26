using System.Collections;
using System.Collections.Generic;
using Unity.Entities.Serialization;
using Unity.Entities;
using UnityEngine;

public class GetPrefabReferenceAuthoring : MonoBehaviour
{
    public GameObject Prefab;
}

public class GetPrefabReferenceBaker : Baker<GetPrefabReferenceAuthoring>
{
    public override void Bake(GetPrefabReferenceAuthoring authoring)
    {
        var entityPrefab = new EntityPrefabReference(authoring.Prefab);
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new EntityPrefabComponent() { Value = entityPrefab });
    }
}
