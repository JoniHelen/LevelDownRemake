using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using LevelDown.Components.Tags;

namespace LevelDown.Components.Authoring
{
    public class FloorAuthoring : MonoBehaviour { }

    public class FloorBaker : Baker<FloorAuthoring>
    {
        public override void Bake(FloorAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<RandomValue>(entity);
            AddComponent<Initialized>(entity);
            SetComponentEnabled<Initialized>(entity, false);
            AddComponent(entity, new PhysicsMassOverride { IsKinematic = 1 });
            AddComponent(entity, new Floor { Tall = false });
            AddComponent(entity, new PostTransformMatrix { Value = float4x4.identity });
            AddComponent(entity, new Shrinking { Duration = 0.5f });
            SetComponentEnabled<Shrinking>(entity, false);
        }
    }
}