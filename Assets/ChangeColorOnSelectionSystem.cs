using Unity.Entities;
using Unity.Rendering;

public partial struct ChangeColorOnSelectionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (urpColorBase, selectable, entity) in SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>, RefRO<SelectableComponentData>>()
            .WithPresent<SelectableComponentData>()
            .WithEntityAccess())
        {
            urpColorBase.ValueRW.Value = SystemAPI.IsComponentEnabled<SelectableComponentData>(entity)?selectable.ValueRO.selectedColor:selectable.ValueRO.unselectedColor;
        }
    }
}