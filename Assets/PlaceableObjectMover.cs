using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlaceableObjectMover : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private BaseEditMode editMode;
    [SerializeField] private Grid baseGrid;
    [SerializeField] private BaseGridManager gridManager;
    [SerializeField] private Camera mainCamera;

    [Header("Placement Settings")]
    [SerializeField] private Vector2Int gridSize = Vector2Int.one;
    [SerializeField] private bool canStore = true;
    [SerializeField] private GameObject selectionHighlight;
    [SerializeField] private PlaceableItem itemDefinition;
    [SerializeField]
    private PlaceableInventoryManager inventoryManager;

    private Collider2D objectCollider;
    private bool isSelected;

    private static PlaceableObjectMover currentlySelected;

    public bool IsSelected => isSelected;
    public static PlaceableObjectMover CurrentSelection =>
        currentlySelected;
    public PlaceableItem ItemDefinition =>
    itemDefinition;

    private Vector3Int bottomLeftCell;
    private bool isMoving;
    private bool hasReservation;

    public Vector2Int GridSize => gridSize;
    public bool CanStore => canStore;
    public bool IsMoving => isMoving;
    public Vector3Int BottomLeftCell => bottomLeftCell;

    private void Awake()
    {
        objectCollider = GetComponent<Collider2D>();
        FindSceneReferences();
    }

    private void OnEnable()
    {
        FindSceneReferences();

        if (editMode != null)
            editMode.EditModeChanged += OnEditModeChanged;
    }

    private void Start()
    {
        FindSceneReferences();

        if (baseGrid == null || gridManager == null)
        {
            Debug.LogError(
                $"{gameObject.name} is missing grid references."
            );

            return;
        }

        bottomLeftCell = CalculateBottomLeftCell();

        hasReservation =
            gridManager.TryOccupyArea(
                bottomLeftCell,
                gridSize
            );
    }

    private void OnDisable()
    {
        if (editMode != null)
            editMode.EditModeChanged -= OnEditModeChanged;
    }

    private void OnEditModeChanged(bool editing)
    {
        if (!editing && isMoving)
        {
            if (!hasReservation)
            {
                hasReservation =
                    gridManager.TryOccupyArea(
                        bottomLeftCell,
                        gridSize
                    );
            }

            isMoving = false;
        }
    }

    private void Update()
    {
        if (editMode == null ||
            !editMode.IsEditing ||
            mainCamera == null ||
            baseGrid == null ||
            gridManager == null)
        {
            return;
        }

        if (Mouse.current == null)
            return;

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    -mainCamera.transform.position.z
                )
            );

        mouseWorldPosition.z = 0f;

        if (isMoving)
        {
            MoveToMouse(mouseWorldPosition);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame &&
            !IsPointerOverUI())
        {
            HandleClick(mouseWorldPosition);
        }
    }

    private void MoveToMouse(Vector3 mouseWorldPosition)
    {
        Vector3Int candidateCell =
            CalculateBottomLeftCell(mouseWorldPosition);

        if (candidateCell == bottomLeftCell)
            return;

        if (!gridManager.IsAreaAvailable(
                candidateCell,
                gridSize))
        {
            return;
        }

        bottomLeftCell = candidateCell;

        transform.position =
            gridManager.AreaToWorld(
                bottomLeftCell,
                gridSize
            );
    }

    private void HandleClick(Vector3 mouseWorldPosition)
    {
        if (!isMoving)
        {
            if (objectCollider != null &&
                objectCollider.OverlapPoint(mouseWorldPosition))
            {
                if (!isSelected)
                {
                    SelectObject();
                }
                else
                {
                    BeginMoving();
                }
            }

            return;
        }

        bool placed =
            gridManager.TryOccupyArea(
                bottomLeftCell,
                gridSize
            );

        if (placed)
        {
            hasReservation = true;
            isMoving = false;
        }
    }

    private Vector3Int CalculateBottomLeftCell()
    {
        Vector3 centerOffset =
            GetCenterOffset();

        return baseGrid.WorldToCell(
            transform.position - centerOffset
        );
    }

    private Vector3Int CalculateBottomLeftCell(
        Vector3 worldPosition)
    {
        Vector3 centerOffset =
            GetCenterOffset();

        return baseGrid.WorldToCell(
            worldPosition - centerOffset
        );
    }

    private Vector3 GetCenterOffset()
    {
        Vector3 cellSize = baseGrid.cellSize;

        return new Vector3(
            (gridSize.x - 1) * cellSize.x * 0.5f,
            (gridSize.y - 1) * cellSize.y * 0.5f,
            0f
        );
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }

    private void FindSceneReferences()
    {
        if (editMode == null)
            editMode = FindAnyObjectByType<BaseEditMode>();

        if (baseGrid == null)
            baseGrid = FindAnyObjectByType<Grid>();

        if (gridManager == null)
            gridManager = FindAnyObjectByType<BaseGridManager>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (inventoryManager == null)
        {
            inventoryManager =
                FindAnyObjectByType<PlaceableInventoryManager>();
        }
    }

    private void SelectObject()
    {
        if (currentlySelected != null &&
            currentlySelected != this)
        {
            currentlySelected.DeselectObject();
        }

        currentlySelected = this;
        isSelected = true;

        if (selectionHighlight != null)
            selectionHighlight.SetActive(true);
    }

    public void DeselectObject()
    {
        isSelected = false;

        if (currentlySelected == this)
            currentlySelected = null;

        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);
    }

    private void BeginMoving()
    {
        if (hasReservation)
        {
            gridManager.ReleaseArea(
                bottomLeftCell,
                gridSize
            );

            hasReservation = false;
        }

        isMoving = true;
    }

    public void StoreForTesting()
    {
        if (!canStore)
            return;

        if (hasReservation)
        {
            gridManager.ReleaseArea(
                bottomLeftCell,
                gridSize
            );

            hasReservation = false;
        }

        if (inventoryManager != null &&
            itemDefinition != null)
        {
            inventoryManager.AddItem(itemDefinition);
        }

        DeselectObject();
        Destroy(gameObject);
    }

    public void ResetEditState()
    {
        isMoving = false;

        if (currentlySelected == this)
        {
            DeselectObject();
            currentlySelected = null;
        }

        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);
    }

    public void SetItemDefinition(PlaceableItem item)
    {
        itemDefinition = item;
    }
}