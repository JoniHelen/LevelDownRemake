using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using LevelDown.Components.Singletons;
using LevelDown.Input;

namespace LevelDown.Components.Aspects
{
    /// <summary>
    /// This aspect controls the player's movement.
    /// </summary>
    public readonly partial struct PlayerControlAspect : IAspect
    {
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRW<PlayerInputData> _playerInput;
        private readonly RefRW<PhysicsVelocity> _velocity;

        public float2 MovementDirection
        {
            get => _playerInput.ValueRO.MovementDirection;
        }

        public InputButton FireButton
        {
            get => _playerInput.ValueRO.FireButton;
        }

        public float2 AimDirection
        {
            get => _playerInput.ValueRO.AimDirection;
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

        /// <summary>
        /// Updates the player's movement velocity from input.
        /// </summary>
        public void Update()
        {
            Velocity = InputLength * MovementSpeed * new float3(MovementDirection, 0);
        }
    }
}