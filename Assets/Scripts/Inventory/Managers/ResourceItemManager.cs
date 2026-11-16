using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ResourceStack
{
    public ResourceItem resourceItem;
    public int amount;
}

public class ResourceItemManager : MonoBehaviour
{
    public List<ResourceItem> resourceItems = new List<ResourceItem>();

    public bool doUpdateDisplay = false;

    public List<ResourceStack> inventory = new List<ResourceStack>();

    private void Awake()
    {
        ResourceItem[] loadedItems =
            Resources.LoadAll<ResourceItem>("");

        resourceItems.AddRange(loadedItems);

        Debug.Log(
            "Loaded resource items: " +
            resourceItems.Count
        );
    }

    public ResourceItem GetResourceItemByName(string itemName)
    {
        return resourceItems.Find(item => item.itemName == itemName);
    }

    public void AddResource(ResourceItem resourceItem, int amount)
    {
        ResourceStack existingStack =
            inventory.Find(
                stack => stack.resourceItem == resourceItem
            );

        if (existingStack != null)
        {
            existingStack.amount += amount;
        }
        else
        {
            inventory.Add(
                new ResourceStack
                {
                    resourceItem = resourceItem,
                    amount = amount
                }
            );
        }

        // Update the quantity used by ResourceEntryUI
        resourceItem.quantity += amount;

        // Tell ResourceDisplay to regenerate its entries
        doUpdateDisplay = true;

        Debug.Log(
            "Added " +
            amount + " " +
            resourceItem.itemName +
            ". New quantity: " +
            resourceItem.quantity
        );
    }
}
