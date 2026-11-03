using UnityEngine;
using System.Collections.Generic;

public class HotbarManager : MonoBehaviour
{
    public List<GameObject> childList = new List<GameObject>();

    public List<HotbarItem> hotbarItems = new List<HotbarItem>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            childList.Add(child.gameObject);
        }

        Debug.Log($"HotbarManager: Found {childList.Count} child objects.");

        for (int i = 0; i < childList.Count; i++)
        {
            hotbarItems.Add(null);
        }

        //wait a few seconds and then add a test item to the first slot
        Invoke("AddTestItem", 2f);
    }

    void AddTestItem()
    {
        //test item is an object called French Fries in the Resources folder
        // The full path is Assets/Inventory/Consumables/French Fries/French Fries.asset
        HotbarItem testItem = Resources.Load<HotbarItem>("InventoryItems/HotbarItems/French Fries");

        setHotbarItem(0, testItem);
    }

    void setHotbarItem(int slotIndex, HotbarItem item)
    {
        if (slotIndex < 0 || slotIndex >= childList.Count)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        hotbarItems[slotIndex] = item;

        Debug.Log($"HotbarManager: Set item in slot {slotIndex} to {item?.itemName ?? "None"}");

        updateSlotUI(slotIndex);
    }

    void removeHotbarItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= childList.Count)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        hotbarItems[slotIndex] = null;

        Debug.Log($"HotbarManager: Removed item from slot {slotIndex}");

        updateSlotUI(slotIndex);
    }

    HotbarItem getHotbarItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hotbarItems.Count)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return null;
        }

        var item = hotbarItems[slotIndex];
        Debug.Log($"HotbarManager: Item in slot {slotIndex} is {item?.itemName ?? "None"}");
        return item;
    }

    void updateSlotUI(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= childList.Count)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        var item = hotbarItems[slotIndex];
        
        // each child of the hotbar has a child with a raw image component, if there is no item then set the image to none and opacity of color to 0, if there is an item then set the image to the item's icon and opacity of color to 1
        var slotUI = childList[slotIndex].transform.GetChild(0).GetComponent<UnityEngine.UI.RawImage>();
        if (item == null)
        {
            slotUI.texture = null;
            slotUI.color = new Color(1, 1, 1, 0);
        }
        else
        {
            slotUI.texture = item.icon.texture;
            slotUI.color = new Color(1, 1, 1, 1);
        }
    }
}