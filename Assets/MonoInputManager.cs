using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class MonoInputManager : MonoBehaviour
{
    private EntityManager _entityManager;
    private Entity _mouseInputSingleton;
    private UnityEngine.Plane _raycastPlane;
    private Vector3 _startMousePosition;
    private Vector3 _endMousePosition;
    private bool _isCurrentlySelecting;
    [SerializeField] private UISelectionDrawingView _rectDrawer;

    // Start is called before the first frame update
    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _mouseInputSingleton = _entityManager.CreateSingleton<MouseInputComponentData>();
        _raycastPlane = new UnityEngine.Plane(Vector3.up, Vector3.zero);

        var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithPresent<SelectableComponentData>().Build(_entityManager);
        var entities = entityQuery.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities)
        {
            _entityManager.SetComponentEnabled<SelectableComponentData>(entity,false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_isCurrentlySelecting)
        {
            var currentMousePosition = Input.mousePosition;
            _rectDrawer.Draw(_startMousePosition,currentMousePosition);
        }
        if (Input.GetMouseButtonDown(0))
        {
            //Start Selection Process
            _isCurrentlySelecting = true;
            _startMousePosition = Input.mousePosition;
            Debug.Log(_startMousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            //End Selection Process
            _isCurrentlySelecting = false;
            DeselectAllEntities();
            _rectDrawer.EndDraw();
            _endMousePosition = Input.mousePosition;
            var rect = GenerateRect(_startMousePosition, _endMousePosition);
            if (rect.height + rect.width < 40) SelectWithRaycast(_endMousePosition);
            else SelectMultiple(rect);
        }

        if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (_raycastPlane.Raycast(ray, out var hitDistance))
            {
                var position = ray.GetPoint(hitDistance);
                var query = new EntityQueryBuilder(Allocator.Temp).WithAll<MovementComponentData>().WithAll<SelectableComponentData>().Build(_entityManager);
                var entities = query.ToEntityArray(Allocator.Temp);
                
                for (var i = 0; i < entities.Length; i++)
                {
                    var moveData = _entityManager.GetComponentData<MovementComponentData>(entities[i]);
                    moveData.TargetPosition = (float3)position;
                    moveData.allowMovement = true;
                    _entityManager.SetComponentData(entities[i], moveData); 
                }
                var data = _entityManager.GetComponentData<MouseInputComponentData>(_mouseInputSingleton);
                data.mousePositionOnHorizontalPlane = ray.GetPoint(hitDistance);
                _entityManager.SetComponentData(_mouseInputSingleton, data);
            }
        }
    }

    private void SelectMultiple(Rect rect)
    {
        var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform>().Build(_entityManager);
        var entities = entityQuery.ToEntityArray(Allocator.Temp);
        var transforms = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);


        for (var i = 0; i < transforms.Length; i++)
        {
            var entityScreenPosition = Camera.main.WorldToScreenPoint(transforms[i].Position);
            if (rect.Contains(entityScreenPosition))
            {
                _entityManager.SetComponentEnabled<SelectableComponentData>(entities[i], true);
            }
        }
    }

    private void DeselectAllEntities()
    {
        var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithPresent<SelectableComponentData>().Build(_entityManager);
        var entities = entityQuery.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities)
        {
            _entityManager.SetComponentEnabled<SelectableComponentData>(entity,false);
        }
        
    }

    private void SelectWithRaycast(Vector3 endMousePosition)
    {
        var query = _entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
        var singleton = query.GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = singleton.CollisionWorld;

        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var raycastInput = new RaycastInput
        {
            Start = cameraRay.origin,
            End = cameraRay.GetPoint(100),
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << 6,
                GroupIndex = 0,
            }
        };

        if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
        {
            if (_entityManager.HasComponent<SelectableComponentData>(hit.Entity))
            {
                _entityManager.SetComponentEnabled<SelectableComponentData>(hit.Entity, true);
            }
        }
    }

    private Rect GenerateRect(Vector3 startMousePosition, Vector3 endMousePosition)
    {
        var bottomLeftCorner = new Vector2(Mathf.Min(startMousePosition.x, endMousePosition.x), Mathf.Min(startMousePosition.y, endMousePosition.y));
        var width = Mathf.Abs(startMousePosition.x - endMousePosition.x);
        var height = Mathf.Abs(startMousePosition.y - endMousePosition.y);

        return new Rect(bottomLeftCorner, new Vector2(width, height));
    }
}

