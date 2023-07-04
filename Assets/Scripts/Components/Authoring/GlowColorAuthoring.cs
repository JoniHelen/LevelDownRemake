using Unity.Entities;
using UnityEngine;

public class GlowColorAuthoring : MonoBehaviour
{
    public FloorSettings floorSettings;
    public Color color = Color.white;
    public float value = 1f;
}

public class GlowColorBaker : Baker<GlowColorAuthoring>
{
    public override void Bake(GlowColorAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new GlowColor
        {
            Color = authoring.color
        });

        AddComponent(entity, new GlowBrightness {
            Value = authoring.value
        });

        AddComponent(entity, new ColorFlash
        {
            Finished = true,
            BaseColor = Color.white,
            Duration = 0.3f
        });
    }
}