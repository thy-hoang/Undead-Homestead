using UnityEngine;

public class PlaceablePlacementController : MonoBehaviour
{
    [SerializeField] private BaseEditMode editMode;
    [SerializeField] private BaseGridManager gridManager;
    [SerializeField] private Transform placedObjectsParent;
    [SerializeField]
    private PlaceableInventoryManager inventoryManager;

    public void PlaceItem(PlaceableItem item)
    {
        if (item == null || item.prefab == null)
        {
            Debug.LogWarning("Placeable item or prefab is missing.");
            return;
        }

        if (editMode == null || !editMode.IsEditing)
        {
            Debug.LogWarning(
                "Placeable items can only be added during edit mode."
            );

            return;
        }

        if (inventoryManager == null)
        {
            Debug.LogWarning(
                "PlaceableInventoryManager is not assigned."
            );

            return;
        }

        if (!inventoryManager.TryConsume(item))
        {
            Debug.LogWarning(
                $"No {item.itemName} items are available."
            );

            return;
        }

        if (!gridManager.TryFindFirstAvailableArea(
                item.gridSize,
                out Vector3Int bottomLeft))
        {
            Debug.LogWarning(
                $"No space available for {item.itemName}."
            );

            return;
        }

        Vector3 spawnPosition =
            gridManager.AreaToWorld(
                bottomLeft,
                item.gridSize
            );

        GameObject placedObject = Instantiate(
            item.prefab,
            spawnPosition,
            Quaternion.identity,
            placedObjectsParent
        );

        PlaceableObjectMover mover =
            placedObject.GetComponent<PlaceableObjectMover>();

        if (mover != null)
        {
            mover.SetItemDefinition(item);
        }
        else
        {
            inventoryManager.AddItem(item);
            Destroy(placedObject);

            Debug.LogWarning(
                $"{placedObject.name} is missing PlaceableObjectMover."
            );
        }

        Debug.Log(
            $"Placed {item.itemName}: {placedObject.name}"
        );
    }
}