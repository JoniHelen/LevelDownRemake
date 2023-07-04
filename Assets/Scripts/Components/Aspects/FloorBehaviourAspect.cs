using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

public readonly partial struct FloorBehaviourAspect : IAspect
{
    public readonly Entity Self;

    private readonly RefRW<PostTransformMatrix> _Transform;
    private readonly ColorFlashAspect _Flash;
    private readonly RefRO<PhysicsCollider> _PhysicsCollider;

    public bool Tall
    {
        get => _Flash.Tall;
        set => _Flash.Tall = value;
    }

    public float3 Position
    {
        get => _Transform.ValueRO.Value.Translation();
        set
        {
            _Transform.ValueRW.Value.c3 = new float4(value, 1);
            unsafe
            {
                BoxCollider* boxPtr = (BoxCollider*)_PhysicsCollider.ValueRO.ColliderPtr;
                BoxGeometry geometry = boxPtr->Geometry;
                geometry.Center = _Transform.ValueRO.Value.Translation();
                boxPtr->Geometry = geometry;
            }
        }
    }

    public float3 Scale
    {
        get => _Transform.ValueRO.Value.Scale();
        set
        {
            _Transform.ValueRW.Value.c0.x = value.x;
            _Transform.ValueRW.Value.c1.y = value.y;
            _Transform.ValueRW.Value.c2.z = value.z;
        }
    }

    public float4x4 PostMatrix
    {
        get => _Transform.ValueRO.Value;
        set => _Transform.ValueRW.Value = value;
    }

    public void SetHeight(bool tall)
    {
        if (_Flash.Tall == tall) return;

        _Flash.Tall = tall;

        _Flash.Brightness = tall ? 0 : 1.1f;

        float4x4 tallTransform = float4x4.TRS(new float3(0, 0, -0.5f), quaternion.identity, new float3(1, 1, 2));

        _Transform.ValueRW.Value = math.mul(tall ? tallTransform : math.inverse(tallTransform), _Transform.ValueRO.Value);

        unsafe
        {
            BoxCollider* boxPtr = (BoxCollider*)_PhysicsCollider.ValueRO.ColliderPtr;
            BoxGeometry geometry = boxPtr->Geometry;
            geometry.Size = tall ? new float3(1, 1, 2) : 1;
            geometry.Center = _Transform.ValueRO.Value.Translation();
            boxPtr->Geometry = geometry;
        }
    }
}