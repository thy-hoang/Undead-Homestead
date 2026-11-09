using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableItemButton : MonoBehaviour
{
    [SerializeField] private PlaceableItem placeableItem;
    [SerializeField] private PlaceablePlacementController placementController;
    [SerializeField] private PlaceableInventoryManager inventoryManager;
    [SerializeField] private TMP_Text quantityText;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(PlaceSelectedItem);
    }

    private void Start()
    {
        if (inventoryManager == null)
        {
            inventoryManager =
                FindAnyObjectByType<PlaceableInventoryManager>();
        }

        if (inventoryManager != null)
        {
            inventoryManager.InventoryChanged += RefreshDisplay;
        }

        RefreshDisplay();
    }

    private void OnDestroy()
    {
        if (inventoryManager != null)
        {
            inventoryManager.InventoryChanged -= RefreshDisplay;
        }
    }

    private void PlaceSelectedItem()
    {
        if (placementController != null &&
            placeableItem != null)
        {
            placementController.PlaceItem(placeableItem);
        }
    }

    private void RefreshDisplay()
    {
        if (inventoryManager == null ||
            placeableItem == null)
        {
            return;
        }

        int quantity =
            inventoryManager.GetQuantity(placeableItem);

        if (quantityText != null)
            quantityText.text = quantity.ToString();

        if (button != null)
            button.interactable = quantity > 0;
    }
}