using System.Collections;
using System.Collections.Generic;
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

        AddComponent(entity, new GlowColorComponent
        {
            Color = authoring.color
        });

        AddComponent(entity, new GlowBrightnessComponent {
            Value = authoring.value
        });

        AddComponent(entity, new ColorFlashComponent
        {
            Finished = true
        });
    }
}