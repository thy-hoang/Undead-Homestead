using UnityEngine;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{

    public ConsumableItem consumableItem;

    [SerializeField] private GameObject ingredientPrefab; // prefab for the ingredient UI element

    private Button myButton;

    private ResourceItemManager resourceItemManager;

    private HotbarManager hotbarManager;

    // Start is called before the first frame update
    void Start()
    {
        Transform ingredientsContainerTransform = transform.Find("IngredientsSection/IngredientsContainer");

        resourceItemManager = FindObjectOfType<ResourceItemManager>();

        hotbarManager = FindObjectOfType<HotbarManager>();

        foreach (RecipeIngredient ingredient in consumableItem.recipe)
        {
            // Instantiate a new ingredient UI element for each ingredient
            GameObject ingredientInstance = Instantiate(ingredientPrefab, ingredientsContainerTransform);

            // set the icon of the ingredient to the icon of the resource item that matches the ingredient's name
            ResourceItem resourceItem = resourceItemManager.GetResourceItemByName(ingredient.item.itemName);
            if (resourceItem != null)
            {
                UnityEngine.UI.RawImage ingredientIcon = ingredientInstance.transform.Find("IngredientIcon")?.GetComponent<UnityEngine.UI.RawImage>();
                if (ingredientIcon != null)
                {
                    ingredientIcon.texture = resourceItem.icon.texture;
                }
                else
                {
                    Debug.LogError($"RecipeButton: No RawImage component found in {ingredientInstance.name}");
                }
            }
            else
            {
                Debug.LogError($"RecipeButton: No ResourceItem found with name {ingredient.item.itemName}");
            }

            // set the text to the quantity
            TMPro.TextMeshProUGUI ingredientAmount = ingredientInstance.transform.Find("IngredientAmount")?.GetComponent<TMPro.TextMeshProUGUI>();
            if (ingredientAmount != null)
            {
                ingredientAmount.text = ingredient.quantity.ToString();
            }
            else
            {
                Debug.LogError($"RecipeButton: No Text component found in {ingredientInstance.name}");
            }

            // if the item is the last item in the recipe, hide the "plus" text element
            if (ingredient == consumableItem.recipe[consumableItem.recipe.Count - 1])
            {
                Transform plusTextTransform = ingredientInstance.transform.Find("Plus");
                if (plusTextTransform != null)
                {
                    plusTextTransform.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError($"RecipeButton: No PlusText object found in {ingredientInstance.name}");
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
            Debug.LogError($"RecipeButton: No Button component found in {gameObject.name}");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnButtonClick()
    {
        // method called to handle cooking the item when the button is clicked
        
        // check if the player has all of the ingredients for the recipe
        bool hasAllIngredients = true;
        foreach (RecipeIngredient ingredient in consumableItem.recipe)
        {
            if (resourceItemManager.GetResourceItemByName(ingredient.item.itemName).quantity < ingredient.quantity)
            {
                hasAllIngredients = false;
                break;
            }
        }

        // remove the ingredients from the player's inventory if they have all of the ingredients
        if (hasAllIngredients)
        {
            foreach (RecipeIngredient ingredient in consumableItem.recipe)
            {
                ResourceItem resourceItem = resourceItemManager.GetResourceItemByName(ingredient.item.itemName);
                if (resourceItem != null)
                {
                    resourceItem.quantity -= ingredient.quantity;
                }
                else
                {
                    Debug.LogError($"RecipeButton: No ResourceItem found with name {ingredient.item.itemName}");
                }
            }

            resourceItemManager.doUpdateDisplay = true;

            // add the cooked item to the player's inventory
            hotbarManager.addItemInNextAvailableSlot(consumableItem);
        }
        else
        {
            Debug.Log("You do not have all of the ingredients for this recipe.");
        }
    }
}
