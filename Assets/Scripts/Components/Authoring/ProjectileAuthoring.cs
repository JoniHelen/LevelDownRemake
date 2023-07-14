using Unity.Entities;
using UnityEngine;

namespace LevelDown.Components.Authoring
{
    public class ProjectileAuthoring : MonoBehaviour
    {

    }

    public class ProjectileBaker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<Projectile>(entity);
            AddComponent(entity, new ColorExplosion
            {
                Duration = 0.1f,
                TargetSize = 1.5f
            });
            SetComponentEnabled<ColorExplosion>(entity, false);
            AddBuffer<EntityBufferData>(entity);
        }
    }
}