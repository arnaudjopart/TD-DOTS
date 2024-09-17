using Unity.Entities;

public partial struct SetTargetPositionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MouseInputComponentData>();
    }
    public void OnUpdate(ref SystemState state)
    {
        var targetPosition = SystemAPI.GetSingleton<MouseInputComponentData>().mousePositionOnHorizontalPlane;

        foreach(var (targetPositionData, selected) in SystemAPI.Query<RefRW<MovementComponentData>, RefRO<SelectableComponentData>>())
        {
            //targetPositionData.ValueRW.TargetPosition = targetPosition;
            //targetPositionData.ValueRW.allowMovement = true;
        }
    }
}
