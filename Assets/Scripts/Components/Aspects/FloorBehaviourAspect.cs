using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public readonly partial struct FloorBehaviourAspect : IAspect
{
    public readonly Entity Self;

    private readonly RefRW<PostTransformMatrix> _Transform;
    private readonly RefRW<FloorComponent> _Floor;

    public bool Tall
    {
        get => _Floor.ValueRO.Tall;
        set => _Floor.ValueRW.Tall = value;
    }

    public void SetHeight(bool tall)
    {
        if (_Floor.ValueRO.Tall == tall) return;

        _Floor.ValueRW.Tall = tall;

        if (tall)
            _Transform.ValueRW.Value = float4x4.TRS(new float3(0, 0, -0.5f), quaternion.identity, new float3(1, 1, 2));
        else
            _Transform.ValueRW.Value = float4x4.identity;
    }
}
