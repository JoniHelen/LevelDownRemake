using Unity.Entities;
using UnityEngine;

namespace LevelDown.Components.Authoring
{
    public class GetPrefabAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
    }

    public class GetPrefabBaker : Baker<GetPrefabAuthoring>
    {
        public override void Bake(GetPrefabAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var entityPrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntityPrefab { Value = entityPrefab });
        }
    }
}