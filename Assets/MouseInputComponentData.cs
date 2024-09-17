using Unity.Entities;
using Unity.Mathematics;

public struct MouseInputComponentData : IComponentData
{
    public float3 mousePositionOnHorizontalPlane;
}