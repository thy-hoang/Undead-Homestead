using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlaceableObjectMover : MonoBehaviour
{
    [SerializeField] private BaseEditMode editMode;
    [SerializeField] private Grid baseGrid;
    [SerializeField] private BaseGridManager gridManager;
    [SerializeField] private Camera mainCamera;

    private Collider2D plotCollider;
    private bool isMoving;
    private Vector3Int currentCell;

    private void Awake()
    {
        plotCollider = GetComponent<Collider2D>();

        if (editMode == null)
            editMode = FindFirstObjectByType<BaseEditMode>();

        if (baseGrid == null)
            baseGrid = FindFirstObjectByType<Grid>();

        if (gridManager == null)
            gridManager = FindFirstObjectByType<BaseGridManager>();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        currentCell = baseGrid.WorldToCell(transform.position);

        if (!gridManager.TryOccupyCell(currentCell))
        {
            Debug.LogWarning($"{gameObject.name} could not reserve its starting cell.");
        }
    }

    private void OnEnable()
    {
        if (editMode != null)
        {
            editMode.EditModeChanged += OnEditModeChanged;
        }
    }

    private void OnDisable()
    {
        if (editMode != null)
        {
            editMode.EditModeChanged -= OnEditModeChanged;
        }
    }

    private void OnEditModeChanged(bool editing)
    {
        if (!editing && isMoving)
        {
            gridManager.TryOccupyCell(currentCell);
            isMoving = false;
        }
    }

    private void Update()
    {
        if (editMode == null || !editMode.IsEditing)
            return;

        if (Mouse.current == null || mainCamera == null ||
            baseGrid == null || gridManager == null)
            return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                -mainCamera.transform.position.z
            )
        );

        mouseWorldPosition.z = 0f;

        if (isMoving)
        {
            Vector3Int newCell = baseGrid.WorldToCell(mouseWorldPosition);

            if (gridManager.IsInsideBase(newCell) &&
                gridManager.IsCellAvailable(newCell))
            {
                currentCell = newCell;
                transform.position = baseGrid.GetCellCenterWorld(currentCell);
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame &&
            !IsPointerOverUI())
        {
            if (!isMoving)
            {
                if (plotCollider != null &&
                    plotCollider.OverlapPoint(mouseWorldPosition))
                {
                    gridManager.ReleaseCell(currentCell);
                    isMoving = true;
                }
            }
            else
            {
                gridManager.TryOccupyCell(currentCell);
                isMoving = false;
            }
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }
}