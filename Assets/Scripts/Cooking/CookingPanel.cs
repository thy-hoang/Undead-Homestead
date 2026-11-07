using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CookingPanel : MonoBehaviour
{

    public List<ConsumableItem> consumableItems = new List<ConsumableItem>();

    [SerializeField] private GameObject buttonPrefab;

    //export a game object that will be used as a parent for the buttons that will be created for each consumable item
    [SerializeField] private GameObject buttonParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //gameObject.SetActive(false);

        ConsumableItem[] loadedItems = Resources.LoadAll<ConsumableItem>("");

        consumableItems.AddRange(loadedItems);

        Debug.Log($"CookingPanel: Loaded {consumableItems.Count} consumable items.");

        // create the recipie button prefab as a child for each consumable item
        foreach (ConsumableItem item in consumableItems)
        {
            GameObject buttonInstance = Instantiate(buttonPrefab, buttonParent.transform);
            buttonInstance.name = item.itemName + "Button";

            // set the button's text to the item's name
            TMPro.TextMeshProUGUI buttonText = buttonInstance.transform.Find("FoodSection/FoodInfo/Name")?.GetComponent<TMPro.TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = item.itemName;
            }
            else
            {
                Debug.LogError($"CookingPanel: No Text component found in {buttonInstance.name}");
            }

            // set the button's stats text to the item's health restored value
            TMPro.TextMeshProUGUI buttonStatsText = buttonInstance.transform.Find("FoodSection/FoodInfo/Stats")?.GetComponent<TMPro.TextMeshProUGUI>();
            if (buttonStatsText != null)
            {
                buttonStatsText.text = item.healthRestored.ToString() + " HP";
            }
            else
            {
                Debug.LogError($"CookingPanel: No Stats component found in {buttonInstance.name}");
            }

            // set the button's image to the item's icon
            // this is a raw image not image, so we need to get the raw image component and set the texture to the item's icon
            UnityEngine.UI.RawImage buttonImage = buttonInstance.transform.Find("FoodSection/FoodIcon")?.GetComponent< UnityEngine.UI.RawImage>();
            if (buttonImage != null)
            {
                if (item.icon != null)
                {
                    buttonImage.texture = item.icon.texture;
                }
                else
                {
                    Debug.LogError($"CookingPanel: Item {item.itemName} has no icon assigned.");
                }
            }
            else
            {
                Debug.LogError($"CookingPanel: No Image component found in {buttonInstance.name}");
            }

            // set recipie for recipe button
            RecipeButton recipeButton = buttonInstance.GetComponent<RecipeButton>();
            if (recipeButton != null)
            {
                recipeButton.consumableItem = item;
            }
            else
            {
                Debug.LogError($"CookingPanel: No RecipeButton component found in {buttonInstance.name}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
