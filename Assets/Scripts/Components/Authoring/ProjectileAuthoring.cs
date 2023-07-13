using Unity.Entities;
using UnityEngine;

namespace LevelDown.Components
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
        }
    }
}