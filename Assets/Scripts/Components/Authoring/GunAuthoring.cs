using Unity.Entities;
using UnityEngine;
using LevelDown.Components.Tags;

namespace LevelDown.Components.Authoring
{
    public class GunAuthoring : MonoBehaviour
    {

    }

    public class GunBaker : Baker<GunAuthoring>
    {
        public override void Bake(GunAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Gun>(entity);
        }
    }
}