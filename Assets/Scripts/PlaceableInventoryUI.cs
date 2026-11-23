using UnityEngine;

public class PlaceableInventoryUI : MonoBehaviour
{
    [SerializeField] private BaseEditMode editMode;
    [SerializeField] private GameObject inventoryPanel;

    private void Awake()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (editMode != null)
            editMode.EditModeChanged += OnEditModeChanged;
    }

    private void OnDisable()
    {
        if (editMode != null)
            editMode.EditModeChanged -= OnEditModeChanged;
    }

    private void Start()
    {
        if (editMode != null)
            OnEditModeChanged(editMode.IsEditing);
    }

    private void OnEditModeChanged(bool editing)
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(editing);
    }
}