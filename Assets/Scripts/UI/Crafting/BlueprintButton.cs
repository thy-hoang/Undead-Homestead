using UnityEngine;
using UnityEngine.UI;

public class BlueprintButton : MonoBehaviour
{

    public CraftableItem craftableItem; // the craftable item that this button represents

    [SerializeField] private GameObject componentPrefab; // prefab for the component UI element

    private Button myButton;

    private ResourceItemManager resourceItemManager;

    private HotbarManager hotbarManager;

    // Start is called before the first frame update
    void Start()
    {
        Transform componentsContainerTransform = transform.Find("ComponentsSection/ComponentsContainer");

        resourceItemManager = FindAnyObjectByType<ResourceItemManager>();

        hotbarManager = FindAnyObjectByType<HotbarManager>();

        foreach (CraftingComponent component in craftableItem.components)
        {
            // Instantiate a new component UI element for each component
            GameObject componentInstance = Instantiate(componentPrefab, componentsContainerTransform);

            // set the icon of the component to the icon of the resource item that matches the component's name
            ResourceItem resourceItem = resourceItemManager.GetResourceItemByName(component.item.itemName);
            if (resourceItem != null)
            {
                UnityEngine.UI.RawImage componentIcon = componentInstance.transform.Find("ComponentIcon")?.GetComponent<UnityEngine.UI.RawImage>();
                if (componentIcon != null)
                {
                    componentIcon.texture = resourceItem.icon.texture;
                }
                else
                {
                    Debug.LogError($"BlueprintButton: No RawImage component found in {componentInstance.name}");
                }
            }
            else
            {
                Debug.LogError($"BlueprintButton: No ResourceItem found with name {component.item.itemName}");
            }

            // set the text to the quantity
            TMPro.TextMeshProUGUI componentAmount = componentInstance.transform.Find("ComponentAmount")?.GetComponent<TMPro.TextMeshProUGUI>();
            if (componentAmount != null)
            {
                componentAmount.text = component.quantity.ToString();
            }
            else
            {
                Debug.LogError($"BlueprintButton: No Text component found in {componentInstance.name}");
            }

            // if the item is the last item in the recipe, hide the "plus" text element
            if (component == craftableItem.components[craftableItem.components.Count - 1])
            {
                Transform plusTextTransform = componentInstance.transform.Find("Plus");
                if (plusTextTransform != null)
                {
                    plusTextTransform.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError($"BlueprintButton: No PlusText object found in {componentInstance.name}");
                }
            }
        }

        myButton = GetComponent<Button>();
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError($"BlueprintButton: No Button component found in {gameObject.name}");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnButtonClick()
    {
        // method called to handle crafting the item when the button is clicked

        // check if the player has all of the components for the recipe
        bool hasAllComponents = true;
        foreach (CraftingComponent component in craftableItem.components)
        {
            if (resourceItemManager.GetResourceItemByName(component.item.itemName).quantity < component.quantity)
            {
                hasAllComponents = false;
                break;
            }
        }

        // remove the components from the player's inventory if they have all of the components
        if (hasAllComponents)
        {
            foreach (CraftingComponent component in craftableItem.components)
            {
                ResourceItem resourceItem = resourceItemManager.GetResourceItemByName(component.item.itemName);
                if (resourceItem != null)
                {
                    resourceItem.quantity -= component.quantity;
                }
                else
                {
                    Debug.LogError($"RecipeButton: No ResourceItem found with name {component.item.itemName}");
                }
            }

            resourceItemManager.doUpdateDisplay = true;

            // add the crafted item to the player's inventory
            hotbarManager.addItemInNextAvailableSlot(craftableItem);
        }
        else
        {
            Debug.Log("You do not have all of the components for this blueprint.");
        }
    }
}
