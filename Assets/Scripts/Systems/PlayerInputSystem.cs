using Unity.Entities;
using Unity.Mathematics;
using static UnityEngine.Input;

public partial class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach (var input in SystemAPI.Query<RefRW<PlayerInput>>())
        {
            float2 movementInput = new(GetAxisRaw("Horizontal"), GetAxisRaw("Vertical"));
            input.ValueRW.InputLength = math.clamp(math.length(movementInput), 0, 1);
            input.ValueRW.MovementDirection = math.normalizesafe(movementInput);
        }
    }
}
