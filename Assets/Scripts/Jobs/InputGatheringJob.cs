using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using LevelDown.Components.Singletons;

namespace LevelDown.Jobs
{
    public partial struct InputGatheringJob : IJobEntity
    {
        public void Execute(ref PlayerInputData input)
        {
            float2 movementInput = GetMovementInput();
            input.InputLength = math.clamp(math.length(movementInput), 0, 1);
            input.MovementDirection = math.normalizesafe(movementInput);
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
