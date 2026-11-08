using UnityEngine;

public class BaseEditMode : MonoBehaviour
{
    [SerializeField] private GameObject gridOverlay;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float editCameraSize = 8f;

    private float normalCameraSize;
    private bool editMode = false;

    public bool IsEditing => editMode;

    public event System.Action<bool> EditModeChanged;

    private void Awake()
    {
        editMode = false;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null && mainCamera.orthographic)
        {
            normalCameraSize = mainCamera.orthographicSize;
        }
    }

    private void Start()
    {
        ApplyEditModeState(false);
    }

    public void ToggleEditMode()
    {
        editMode = !editMode;

        ApplyEditModeState(editMode);

        EditModeChanged?.Invoke(editMode);
    }

    private void ApplyEditModeState(bool editing)
    {
        if (gridOverlay != null)
        {
            gridOverlay.SetActive(editing);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = !editing;
        }

        if (playerObject != null)
        {
            playerObject.SetActive(!editing);
        }

        if (mainCamera != null && mainCamera.orthographic)
        {
            if (editing)
            {
                mainCamera.orthographicSize = editCameraSize;
            }
            else
            {
                mainCamera.orthographicSize = normalCameraSize;
            }
        }

        if (!editing)
        {
            ResetAllPlaceableObjects();
        }
    }

    private void ResetAllPlaceableObjects()
    {
        PlaceableObjectMover[] movers =
            FindObjectsByType<PlaceableObjectMover>();

        foreach (PlaceableObjectMover mover in movers)
        {
            mover.ResetEditState();
        }
    }
}