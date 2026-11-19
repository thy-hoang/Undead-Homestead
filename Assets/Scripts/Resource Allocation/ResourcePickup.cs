using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    [Header("Resource")]
    [SerializeField] private ResourceItem resourceItem;

    [SerializeField]
    private int amount = 1;

    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        ResourceItemManager resourceItemManager =
            FindAnyObjectByType<ResourceItemManager>();

        if (resourceItemManager == null)
        {
            Debug.LogError(
                "No ResourceItemManager exists in this scene."
            );

            return;
        }

        if (resourceItem == null)
        {
            Debug.LogError(
                "ResourcePickup has no ResourceItem assigned."
            );

            return;
        }

        resourceItemManager.GetResourceItemByName(resourceItem.itemName).quantity += amount;

        resourceItemManager.doUpdateDisplay = true;

        collected = true;

        Debug.Log(
            "Picked up " +
            amount + " " +
            resourceItem.itemName
        );

        Destroy(gameObject);
    }

        public void SetAmount(int newAmount)
    {
        amount = newAmount;
    }
}