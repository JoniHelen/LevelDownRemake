using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class FloorAuthoring : MonoBehaviour
{

}

public class FloorBaker : Baker<FloorAuthoring>
{
    public override void Bake(FloorAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new Floor
        {
            Tall = false
        });

        AddComponent(entity, new PostTransformMatrix
        {
            Value = float4x4.identity
        });

        AddComponent<RandomValue>(entity);

        AddComponent(entity, new Shrinking
        {
            Duration = 0.5f
        });

        SetComponentEnabled<Shrinking>(entity, false);
    }
}