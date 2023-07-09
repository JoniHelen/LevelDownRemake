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
    }
}