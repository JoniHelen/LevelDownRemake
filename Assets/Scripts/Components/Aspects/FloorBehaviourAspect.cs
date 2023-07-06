using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using LevelDown.Components.Singletons;

namespace LevelDown.Components.Aspects
{
    public readonly partial struct FloorBehaviourAspect : IAspect
    {
        public readonly Entity Self;

        private readonly RefRW<PostTransformMatrix> _transform;
        private readonly RefRW<LocalTransform> _local;
        private readonly ColorFlashAspect _flash;
        private readonly RigidBodyAspect _rigidBody;
        private readonly RefRW<PhysicsCollider> _physicsCollider;

        public bool Tall
        {
            get => _flash.Tall;
            set => _flash.Tall = value;
        }

        public float3 Position
        {
            get => _local.ValueRO.Position;
            set => _local.ValueRW.Position = value;
        }

        public quaternion Rotation
        {
            get => _local.ValueRO.Rotation;
            set => _local.ValueRW.Rotation = value;
        }

        public float3 Scale
        {
            get => _transform.ValueRO.Value.Scale();
            set
            {
                _transform.ValueRW.Value.c0.x = value.x;
                _transform.ValueRW.Value.c1.y = value.y;
                _transform.ValueRW.Value.c2.z = value.z;
            }
        }

        public float3 TallScale { get => new(1, 1, 2); }

        public void Reset(FloorPhysicsBlobs blobs)
        {
            SetHeight(false, blobs);
            Scale = 1;
            Position = 0;
            Rotation = quaternion.identity;
            _rigidBody.IsKinematic = true;
            _rigidBody.LinearVelocity = _rigidBody.AngularVelocityLocalSpace = 0;
        }

        public void SetHeight(bool tall, FloorPhysicsBlobs blobs)
        {
            if (Tall == tall) return;

            Tall = tall;
            _flash.Brightness = tall ? 0 : 1.1f;
            Scale = tall ? TallScale : 1;
            Position += tall ? new float3(0, 0, -0.5f) : new(0, 0, 0.5f);
            _physicsCollider.ValueRW.Value = tall ? blobs.Tall : blobs.Small;
        }
    }
}