using System.Collections;
using System.Collections.Generic;
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

        AddComponent(entity, new FloorComponent
        {
            Tall = false
        });

        AddComponent(entity, new PostTransformMatrix
        {
            Value = float4x4.identity
        });
    }
}
