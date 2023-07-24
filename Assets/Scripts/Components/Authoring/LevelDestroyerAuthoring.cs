using LevelDown.Components.Tags;
using Unity.Entities;
using UnityEngine;

namespace LevelDown.Components
{
	public class LevelDestroyerAuthoring : MonoBehaviour
	{
		
	}

	public class LevelDestroyerBaker : Baker<LevelDestroyerAuthoring>
	{
		public override void Bake(LevelDestroyerAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent<LevelDestroyer>(entity);
			AddBuffer<EntityBufferData>(entity);
		}
	}
}