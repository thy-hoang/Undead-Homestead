using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GUIOpener : MonoBehaviour
{
    [SerializeField] private BaseEditMode editMode;
    [SerializeField] private GameObject guiPrefab;
    [SerializeField] private Camera mainCamera;

    private Collider2D objectCollider;
    private GameObject instantiatedGUI;

    private void Awake()
    {
        objectCollider = GetComponent<Collider2D>();

        FindSceneReferences();

        if (guiPrefab != null)
        {
            instantiatedGUI = Instantiate(
                guiPrefab            
                );

            instantiatedGUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (editMode == null ||
            editMode.IsEditing ||
            instantiatedGUI == null ||
            objectCollider == null ||
            mainCamera == null ||
            Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame ||
            IsPointerOverUI())
        {
            return;
        }

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

        if (objectCollider.OverlapPoint(mouseWorldPosition))
        {
            instantiatedGUI.SetActive(true);
        }
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

        if (mainCamera == null)
            mainCamera = Camera.main;
    }
}