using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using LevelDown.Components.Singletons;

namespace LevelDown.Components.Aspects
{
    /// <summary>
    /// This aspect contains components and methods needed to control the floor tiles' behaviour.
    /// </summary>
    public readonly partial struct FloorBehaviourAspect : IAspect
    {
        public readonly Entity Self;

        private readonly RefRW<PostTransformMatrix> _transform;
        private readonly RefRW<LocalTransform> _local;
        private readonly RefRW<Floor> _floor;
        private readonly RefRW<GlowBrightness> _brightness;
        private readonly RigidBodyAspect _rigidBody;
        private readonly RefRW<PhysicsCollider> _physicsCollider;

        public bool Tall
        {
            get => _floor.ValueRO.Tall;
            set => _floor.ValueRW.Tall = value;
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

        /// <summary>
        /// Resets the floor tile for later use.
        /// </summary>
        /// <param name="blobs">Small and Tall colliders</param>
        public void Reset(FloorPhysicsBlobs blobs)
        {
            SetHeight(false, blobs);
            Scale = 1;
            Position = new(0, 0, 35);
            Rotation = quaternion.identity;
            _rigidBody.IsKinematic = true;
            _rigidBody.LinearVelocity = _rigidBody.AngularVelocityLocalSpace = 0;
        }

        /// <summary>
        /// Initializes the floor tile on level creation.
        /// </summary>
        /// <param name="Tall">Should the tile be Tall</param>
        /// <param name="position">The world position of the tile</param>
        /// <param name="blobs">Small and Tall colliders</param>
        public void Initialize(bool Tall, float2 position, FloorPhysicsBlobs blobs)
        {
            Position = new(position, 35);
            SetHeight(Tall, blobs);
            _rigidBody.IsKinematic = false;
            _rigidBody.GravityFactor = -1;
        }

        /// <summary>
        /// Stops the floor tile from falling towards the screen and prepares it for destruction.
        /// </summary>
        public void Stop()
        {
            _rigidBody.IsKinematic = true;
            _rigidBody.LinearVelocity = _rigidBody.AngularVelocityLocalSpace = 0;
            _rigidBody.GravityFactor = 5;
        }

        /// <summary>
        /// Sets the height of the floor tile.
        /// </summary>
        /// <param name="tall">The height of the tile</param>
        /// <param name="blobs">Small and Tall colliders</param>
        public void SetHeight(bool tall, FloorPhysicsBlobs blobs)
        {
            if (Tall == tall) return;

            Tall = tall;
            _brightness.ValueRW.Value = tall ? 0 : 1.1f;
            Scale = tall ? TallScale : 1;
            Position += tall ? new float3(0, 0, -0.5f) : new(0, 0, 0.5f);
            _physicsCollider.ValueRW.Value = tall ? blobs.Tall : blobs.Small;
        }
    }
}