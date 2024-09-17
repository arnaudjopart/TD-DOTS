using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public Color SelectedColor;
    public Color UnselectedColor;
    public float moveSpeed;

    private class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SelectableComponentData
            {
                selectedColor = new float4(authoring.SelectedColor.r, authoring.SelectedColor.g, authoring.SelectedColor.b, authoring.SelectedColor.a),
                unselectedColor = new float4(authoring.UnselectedColor.r, authoring.UnselectedColor.g, authoring.UnselectedColor.b, authoring.UnselectedColor.a)

            });
            AddComponent(entity, new MovementComponentData
            {
                moveSpeed = authoring.moveSpeed,
                allowMovement = false
            });

        }
    }
}


public struct SelectableComponentData : IComponentData, IEnableableComponent
{
    public float4 selectedColor;
    public float4 unselectedColor;
}


public struct MovementComponentData : IComponentData
{
    public float3 TargetPosition;
    public float moveSpeed;
    internal bool allowMovement;
}
