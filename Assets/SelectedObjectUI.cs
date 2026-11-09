using UnityEngine;
using UnityEngine.UI;

public class SelectedObjectUI : MonoBehaviour
{
    [SerializeField] private BaseEditMode editMode;
    [SerializeField] private GameObject selectedObjectPanel;
    [SerializeField] private Button storeButton;
    [SerializeField] private Camera mainCamera;

    private PlaceableObjectMover currentObject;

    private void Awake()
    {
        selectedObjectPanel.SetActive(false);
    }

    private void Update()
    {
        PlaceableObjectMover selectedObject =
            PlaceableObjectMover.CurrentSelection;

        if (selectedObject != currentObject)
        {
            currentObject = selectedObject;
            UpdatePanel();
        }

        if (currentObject == null ||
            editMode == null ||
            !editMode.IsEditing)
        {
            selectedObjectPanel.SetActive(false);
            return;
        }

        Collider2D objectCollider =
            currentObject.GetComponent<Collider2D>();

        if (objectCollider != null)
        {
            Vector3 worldPosition =
                objectCollider.bounds.max;

            selectedObjectPanel.transform.position =
                mainCamera.WorldToScreenPoint(worldPosition);
        }
    }

    private void UpdatePanel()
    {
        if (currentObject == null)
        {
            selectedObjectPanel.SetActive(false);
            return;
        }

        selectedObjectPanel.SetActive(true);

        storeButton.gameObject.SetActive(
            currentObject.CanStore
        );

        storeButton.onClick.RemoveAllListeners();

        storeButton.onClick.AddListener(
            currentObject.StoreForTesting
        );
    }
}