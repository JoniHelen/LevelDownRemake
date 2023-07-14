using Unity.Entities;
using UnityEngine;
using LevelDown.Components.Tags;

namespace LevelDown.Components.Authoring
{
    public class GunArmAuthoring : MonoBehaviour
    {

    }

    public class GunArmBaker : Baker<GunArmAuthoring>
    {
        public override void Bake(GunArmAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<GunArm>(entity);
        }
    }
}