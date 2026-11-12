using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HotbarManager : MonoBehaviour
{
    public List<GameObject> childList = new List<GameObject>();

    public List<HotbarItem> hotbarItems = new List<HotbarItem>();

    [SerializeField] private Sprite selectedSlotBackground;
    [SerializeField] private Sprite unselectedSlotBackground;

    public int selectedSlotIndex = 0;

    public TMPro.TextMeshProUGUI slotText;

    private void Start()
    {
        childList.Clear();

        foreach (Transform child in transform)
        {
            childList.Add(child.gameObject);
        }

        // Remove extra hotbar entries if there are too many
        while (hotbarItems.Count > childList.Count)
        {
            hotbarItems.RemoveAt(hotbarItems.Count - 1);
        }

        // Add empty entries until there is one for every slot
        while (hotbarItems.Count < childList.Count)
        {
            hotbarItems.Add(null);
        }

        // Display items already assigned in the Inspector
        for (int i = 0; i < hotbarItems.Count; i++)
        {
            updateSlotUI(i);
        }

        selectSlot(selectedSlotIndex);
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
            return;

        var item = hotbarItems[slotIndex];

        var slotUI = childList[slotIndex]
            .transform
            .GetChild(0)
            .GetComponent<UnityEngine.UI.RawImage>();

        if (slotUI == null)
        {
            Debug.LogError("ItemRenderer needs a Raw Image component.");
            return;
        }

        if (item == null || item.icon == null)
        {
            slotUI.texture = null;
            slotUI.color = new Color(1, 1, 1, 0);
        }
        else
        {
            slotUI.texture = item.icon.texture;
            slotUI.color = Color.white;
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        for (Key k = Key.Digit1; k <= Key.Digit9; k++)
        {
            if (Keyboard.current[k].wasPressedThisFrame)
            {
                int slotIndex = k - Key.Digit1;
                selectSlot(slotIndex);
                return;
            }
        }

        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            selectSlot(9);
        }
    }

    void selectSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= childList.Count)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        selectedSlotIndex = slotIndex;

        Debug.Log($"HotbarManager: Selected slot {slotIndex}");

        var item = hotbarItems[slotIndex];
        slotText.text = item?.itemName ?? "";

        //update ui for all slots
        for (int i = 0; i < childList.Count; i++)
        {
            var slotUI = childList[i].transform.GetComponent<UnityEngine.UI.RawImage>();
            if (i == selectedSlotIndex)
            {
                slotUI.texture = selectedSlotBackground.texture;

            }
            else
            {   
                slotUI.texture = unselectedSlotBackground.texture;
            }

        }

    }

    public void addItemInNextAvailableSlot(HotbarItem item)
    {
        for (int i = 0; i < hotbarItems.Count; i++)
        {
            if (hotbarItems[i] == null)
            {
                setHotbarItem(i, item);
                return;
            }
        }

        Debug.LogWarning("HotbarManager: No available slot to add the item.");
    }

    public HotbarItem getSelectedItem() 
    {
        return getHotbarItem(selectedSlotIndex);
    }

    public bool consumeSelectedItem()
    {
        if (getHotbarItem(selectedSlotIndex) == null)
            return false;

        removeHotbarItem(selectedSlotIndex);
        return true;
    }
}