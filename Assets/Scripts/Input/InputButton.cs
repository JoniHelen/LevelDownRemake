using UnityEngine.InputSystem.Controls;

namespace LevelDown.Input
{
    /// <summary>
    /// An unmanaged implementation of a button control.
    /// </summary>
    public struct InputButton
    {
        public bool IsPressed;
        public bool WasPressedThisFrame;
        public bool WasReleasedThisFrame;

        public static implicit operator InputButton(ButtonControl button) => new()
        {
            IsPressed = button.isPressed,
            WasPressedThisFrame = button.wasPressedThisFrame,
            WasReleasedThisFrame = button.wasReleasedThisFrame
        };
    }
}