using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using LevelDown.Components.Singletons;

namespace LevelDown.Components.Aspects
{
    public readonly partial struct PlayerControlAspect : IAspect
    {
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRW<PlayerInputData> _playerInput;
        private readonly RefRW<PhysicsVelocity> _velocity;

        public float2 MovementDirection
        {
            get => _playerInput.ValueRO.MovementDirection;
        }

        public float InputLength
        {
            get => _playerInput.ValueRO.InputLength;
        }

        public float MovementSpeed
        {
            get => _playerInput.ValueRO.MovementSpeed;
            set => _playerInput.ValueRW.MovementSpeed = value;
        }

        public float3 Position
        {
            get => _transform.ValueRO.Position;
            set => _transform.ValueRW.Position = value;
        }

        public float3 Velocity
        {
            get => _velocity.ValueRO.Linear;
            set => _velocity.ValueRW.Linear = value;
        }

        public void UpdateVelocity()
        {
            Velocity = InputLength * MovementSpeed * new float3(MovementDirection, 0);
        }
    }
}