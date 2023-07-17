using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using LevelDown.Components.Singletons;
using LevelDown.Components.Managed;
using LevelDown.Input;
using InputDevice = LevelDown.Input.InputDevice;

namespace LevelDown.Systems
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial struct PlayerInputSystem : ISystem
    {
        private float _controllerDeadZoneThreshold;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInputData>();
            state.RequireForUpdate<Dropdown>();

            Cursor.lockState = CursorLockMode.Confined;

            _controllerDeadZoneThreshold = 0.1f;
        }

        public void OnUpdate(ref SystemState state)
        {
            var dropdown = SystemAPI.ManagedAPI.GetSingleton<Dropdown>();

            foreach(var (input, local) in SystemAPI.Query<RefRW<PlayerInputData>, LocalTransform>())
            {
                input.ValueRW.CurrentInputDevice = (InputDevice)dropdown.Value.value;

                float2 movementInput = GetMovementInput(input.ValueRO.CurrentInputDevice);

                // Input length is used with controllers for normalized movement
                var length = math.length(movementInput);
                var processedLength = length < _controllerDeadZoneThreshold ? 0 : length;

                input.ValueRW.InputLength = math.min(processedLength, 1);
                input.ValueRW.MovementDirection = math.normalizesafe(movementInput);
                input.ValueRW.AimDirection = 
                    GetAimDirection(input.ValueRO.CurrentInputDevice, local.Position, input.ValueRO.AimDirection);
                input.ValueRW.FireButton = GetFireButton(input.ValueRO.CurrentInputDevice);
            }
        }

        private InputButton GetFireButton(InputDevice device) => device switch
        {
            InputDevice.Keyboard => Mouse.current.leftButton,
            InputDevice.Gamepad => Gamepad.current.rightTrigger,
            _ => default
        };

    private float2 GetAimDirection(InputDevice device, float3 playerPos, float2 prevDir)
        {
            switch (device)
            {
                case InputDevice.Keyboard:
                    var screenRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadUnprocessedValue());
                    var mouseWPos = Intersect(new float3(0, 0, -1), new float3(0, 0, -1), screenRay.origin, screenRay.direction);
                    return math.normalizesafe((mouseWPos - playerPos).xy);

                case InputDevice.Gamepad:
                    var input = Gamepad.current.rightStick.ReadUnprocessedValue();
                    if (math.lengthsq(input) < _controllerDeadZoneThreshold * _controllerDeadZoneThreshold)
                        return prevDir;
                    else
                        return math.normalizesafe(input);

                default:
                    return 0;
            }
        }

        float3 Intersect(float3 planeP, float3 planeN, float3 rayP, float3 rayD)
        {
            var d = math.dot(planeP, -planeN);
            var t = -(d + math.dot(rayP, planeN)) / math.dot(rayD, planeN);
            return rayP + t * rayD;
        }

        private float2 GetMovementInput(InputDevice device)
        {
            switch (device)
            {
                case InputDevice.Keyboard:
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

                case InputDevice.Gamepad:
                    return Gamepad.current.leftStick.ReadUnprocessedValue();

                default:
                    return 0;
            }
        }
    }
}