using Powers;
using UnityEngine;
using UnityEngine.EventSystems;

public class TreeFertilizer : Power
{
    [Header("Fertilizer Settings")]
    [SerializeField] private float growthSpeedMultiplier = 4f;
    
    private Vector2 mouseScreenPosition;
    private UnityEngine.Camera mainCamera;
    private bool isCasting = false;
    
    private void Start()
    {
        mainCamera = UnityEngine.Camera.main;
        InputManager.instance.onMousePosition += GetMousePos;
    }

    public override void Activate()
    {
        base.Activate();
        InputManager.instance.onLeftMouseButtonPressStarted += ApplyFertilizer;
    }

    private void ApplyFertilizer()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 _mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Collider2D[] _hits = Physics2D.OverlapCircleAll(_mouseWorld, tileRadius);

        foreach (Collider2D _hit in _hits)
        {
            TreeGrowth _tree = _hit.GetComponent<TreeGrowth>();
            if (_tree != null)
            {
                _tree.GrowFaster(growthSpeedMultiplier);
            }
        }

        Deactivate();
    }
    
    public override void Deactivate()
    {
        base.Deactivate();
        InputManager.instance.onLeftMouseButtonPressStarted -= ApplyFertilizer;
    }
    
    private void GetMousePos(Vector2 _position)
    {
        mouseScreenPosition = _position;
    }
    
    public override bool ShouldStartCooldownOnDeactivate()
    {
        return true;
    }
}