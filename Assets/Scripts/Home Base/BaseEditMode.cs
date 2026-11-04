using UnityEngine;

public class BaseEditMode : MonoBehaviour
{
    [SerializeField] private GameObject gridOverlay;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerMovement playerMovement;

    private bool editMode = false;
    public bool IsEditing => editMode;
    public event System.Action<bool> EditModeChanged;

    private void Awake()
    {
        editMode = false;
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
    }
}