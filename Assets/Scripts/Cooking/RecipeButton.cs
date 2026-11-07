using UnityEngine;

public class RecipeButton : MonoBehaviour
{

    public ConsumableItem consumableItem;

    [SerializeField] private GameObject ingredientPrefab; // prefab for the ingredient UI element

    

    // Start is called before the first frame update
    void Start()
    {
        Transform ingredientsContainerTransform = transform.Find("IngredientsSection/IngredientsContainer");

        // Get all of the ingredients for this recipe and print them to the console
        foreach (RecipeIngredient ingredient in consumableItem.recipe)
        {
            // Instantiate a new ingredient UI element for each ingredient
            GameObject ingredientInstance = Instantiate(ingredientPrefab, ingredientsContainerTransform);

            // set the icon of the ingredient to the icon of the resource item that matches the ingredient's name
            ResourceItem resourceItem = FindObjectOfType<ResourceItemManager>().GetResourceItemByName(ingredient.item.itemName);
            if (resourceItem != null)
            {
                UnityEngine.UI.RawImage ingredientIcon = ingredientInstance.transform.Find("IngredientIcon")?.GetComponent<UnityEngine.UI.RawImage>();
                Debug.Log($"RecipeButton: Found RawImage component in {ingredientInstance.name}: {ingredientIcon != null}");
                Debug.Log($"RecipeButton: Found ResourceItem for {ingredient.item.itemName}: {resourceItem != null}");
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
