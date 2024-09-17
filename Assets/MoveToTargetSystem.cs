using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct MoveToTargetSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localTransform, moveData) in SystemAPI.Query < RefRW<LocalTransform>, RefRO<MovementComponentData>>())
        {
            if(moveData.ValueRO.allowMovement==false) continue;
            var direction = math.normalize(moveData.ValueRO.TargetPosition - localTransform.ValueRO.Position);
            var step = direction*moveData.ValueRO.moveSpeed*SystemAPI.Time.DeltaTime;
            localTransform.ValueRW.Position += new float3(step.x, 0, step.z);
        }
    }
}
