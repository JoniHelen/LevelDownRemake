using UnityEngine;
using Unity.Entities;
using LevelDown.Components.Singletons;

namespace LevelDown.Components.Authoring
{
    public class PlayerInputAuthoring : MonoBehaviour
    {
        public float MovementSpeed;
    }

    public class PlayerInputBaker : Baker<PlayerInputAuthoring>
    {
        public override void Bake(PlayerInputAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerInputData
            {
                MovementSpeed = authoring.MovementSpeed
            });
        }
    }
}