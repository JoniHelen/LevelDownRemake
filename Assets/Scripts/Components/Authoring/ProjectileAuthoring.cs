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
            AddComponentObject(entity, new Managed.ParticleComponent { Obj = authoring.effect });
        }
    }
}