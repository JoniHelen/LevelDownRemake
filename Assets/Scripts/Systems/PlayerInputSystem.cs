using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using LevelDown.Components.Singletons;
using LevelDown.Input;

namespace LevelDown.Systems
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial struct PlayerInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state) => state.RequireForUpdate<PlayerInputData>();

        public void OnUpdate(ref SystemState state)
        {
            foreach(var (input, local) in SystemAPI.Query<RefRW<PlayerInputData>, LocalTransform>())
            {
                float2 movementInput = GetMovementInput();
                // Input length is used with controllers for normalized movement
                input.ValueRW.InputLength = math.clamp(math.length(movementInput), 0, 1);
                input.ValueRW.MovementDirection = math.normalizesafe(movementInput);
                input.ValueRW.AimDirection = GetAimDirection(local.Position);
                input.ValueRW.FireButton = new InputButton { WasPressedThisFrame = Mouse.current.leftButton.wasPressedThisFrame };
            }
        }

        private float2 GetAimDirection(float3 playerPos)
        {
            var screenRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadUnprocessedValue());
            var mouseWPos = Intersect(new float3(0, 0, -1), new float3(0, 0, -1), screenRay.origin, screenRay.direction);
            return math.normalizesafe((mouseWPos - playerPos).xy);
        }

        float3 Intersect(float3 planeP, float3 planeN, float3 rayP, float3 rayD)
        {
            var d = math.dot(planeP, -planeN);
            var t = -(d + math.dot(rayP, planeN)) / math.dot(rayD, planeN);
            return rayP + t * rayD;
        }

        private float2 GetMovementInput()
        {
            float2 input = 0;

            if (Keyboard.current[Key.W].isPressed)
                input += new float2(0, 1);

            if (Keyboard.current[Key.A].isPressed)
                input += new float2(-1, 0);

            if (Keyboard.current[Key.S].isPressed)
                input += new float2(0, -1);

            if (Keyboard.current[Key.D].isPressed)
                input += new float2(1, 0);

            return input;
        }
    }
}