using Unity.Entities;
using UnityEngine;

namespace LevelDown.Components.Authoring
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        public GameObject effect;
    }

    public class ProjectileBaker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<Projectile>(entity);
            AddComponent(entity, new ColorExplosion
            {
                TargetSize = 1.5f,
                Duration = 0.1f
            });
            SetComponentEnabled<ColorExplosion>(entity, false);
            AddBuffer<EntityBufferData>(entity);
            AddComponentObject(entity, new Managed.ParticleComponent { Obj = authoring.effect });
        }
    }
}