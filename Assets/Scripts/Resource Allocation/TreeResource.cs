using UnityEngine;

public class TreeResource : MonoBehaviour
{
    [Header("Tree Health")]
    [SerializeField] private int hitsRequired = 3;

    [Header("Wood Drop")]
    [SerializeField] private GameObject woodDropPrefab;
    [SerializeField] private int woodDropAmount = 1;
    [SerializeField] private float dropRadius = 0.25f;

    private int currentHits;

    private void Awake()
    {
        currentHits = 0;
    }

    public void HitWithAxe()
    {
        currentHits++;

        Debug.Log(
            "Tree hit: " +
            currentHits +
            "/" +
            hitsRequired
        );

        if (currentHits >= hitsRequired)
        {
            DropWood();
            Destroy(gameObject);
        }
    }

    private void DropWood()
    {
        if (woodDropPrefab == null)
        {
            Debug.LogError(
                "Tree has no wood drop prefab assigned."
            );
            return;
        }

        Vector2 offset =
            Random.insideUnitCircle * dropRadius;

        Vector3 dropPosition =
            transform.position +
            new Vector3(offset.x, offset.y, 0f);

        GameObject woodDrop =
            Instantiate(
                woodDropPrefab,
                dropPosition,
                Quaternion.identity
            );

        ResourcePickup pickup =
            woodDrop.GetComponent<ResourcePickup>();

        if (pickup != null)
        {
            pickup.SetAmount(woodDropAmount);
        }

        Debug.Log("Tree dropped wood.");
    }
}