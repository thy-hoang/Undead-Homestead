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