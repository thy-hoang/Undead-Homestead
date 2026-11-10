using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CraftingPanel : MonoBehaviour
{

    public List<WeaponItem> weaponItems = new List<WeaponItem>();

    [SerializeField] private GameObject buttonPrefab;

    //export a game object that will be used as a parent for the buttons that will be created for each weapon item
    [SerializeField] private GameObject buttonParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        WeaponItem[] loadedItems = Resources.LoadAll<WeaponItem>("");

        weaponItems.AddRange(loadedItems);

        Debug.Log($"CraftingPanel: Loaded {weaponItems.Count} weapon items.");

        // create the blueprint button prefab as a child for each weapon item
        foreach (WeaponItem item in weaponItems)
        {
            GameObject buttonInstance = Instantiate(buttonPrefab, buttonParent.transform);
            buttonInstance.name = item.itemName + "Button";

            // set the button's text to the item's name
            TMPro.TextMeshProUGUI buttonText = buttonInstance.transform.Find("WeaponSection/WeaponInfo/Name")?.GetComponent<TMPro.TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = item.itemName;
            }
            else
            {
                Debug.LogError($"CraftingPanel: No Text component found in {buttonInstance.name}");
            }

            // set the button's stats text to the item's health restored value
            /*TMPro.TextMeshProUGUI buttonStatsText = buttonInstance.transform.Find("FoodSection/FoodInfo/Stats")?.GetComponent<TMPro.TextMeshProUGUI>();
            if (buttonStatsText != null)
            {
                buttonStatsText.text = item.healthRestored.ToString() + " HP";
            }
            else
            {
                Debug.LogError($"CraftingPanel: No Stats component found in {buttonInstance.name}");
            }*/

            // set the button's image to the item's icon
            UnityEngine.UI.RawImage buttonImage = buttonInstance.transform.Find("WeaponSection/WeaponIcon")?.GetComponent<UnityEngine.UI.RawImage>();
            if (buttonImage != null)
            {
                if (item.icon != null)
                {
                    buttonImage.texture = item.icon.texture;
                }
                else
                {
                    Debug.LogError($"CraftingPanel: Item {item.itemName} has no icon assigned.");
                }
            }
            else
            {
                Debug.LogError($"CraftingPanel: No Image component found in {buttonInstance.name}");
            }

            // set recipe for blueprint button
            BlueprintButton blueprintButton = buttonInstance.GetComponent<BlueprintButton>();
            if (blueprintButton != null)
            {
                blueprintButton.weaponItem = item;
            }
            else
            {
                Debug.LogError($"CraftingPanel: No BlueprintButton component found in {buttonInstance.name}");
            }
        }

        // connect the close button to the onCloseButtonClick method
        Button closeButton = transform.Find("CloseButton")?.GetComponent<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(onCloseButtonClick);
        }
        else
        {
            Debug.LogError($"CraftingPanel: No CloseButton component found in {gameObject.name}");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void onCloseButtonClick()
    {
        if (transform.parent != null)
        {
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"CraftingPanel: No parent found for {gameObject.name}");
        }
    }
}
