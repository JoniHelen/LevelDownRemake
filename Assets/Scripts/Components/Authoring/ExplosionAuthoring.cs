using Unity.Entities;
using UnityEngine;

namespace LevelDown.Components
{
	public class ExplosionAuthoring : MonoBehaviour
	{
		
	}

	public class ExplosionBaker : Baker<ExplosionAuthoring>
	{
		public override void Bake(ExplosionAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddBuffer<EntityBufferData>(entity);
            AddComponent(entity, new ColorExplosion
            {
                Duration = 0.1f,
                TargetSize = 1.5f
            });
            SetComponentEnabled<ColorExplosion>(entity, false);
        }
	}
}