using UnityEngine;

public class RockResource : MonoBehaviour
{
    [Header("Rock Health")]
    [SerializeField] private int hitsRequired = 3;

    [Header("Stone Drop")]
    [SerializeField] private GameObject stoneDropPrefab;
    [SerializeField] private int stoneDropAmount = 1;
    [SerializeField] private float dropRadius = 0.25f;

    private int currentHits;

    public void HitWithPickaxe()
    {
        currentHits++;

        Debug.Log(
            "Rock hit: " +
            currentHits +
            "/" +
            hitsRequired
        );

        if (currentHits >= hitsRequired)
        {
            DropStone();
            Destroy(gameObject);
        }
    }

    private void DropStone()
    {
        if (stoneDropPrefab == null)
        {
            Debug.LogError(
                "Rock has no stone drop prefab assigned."
            );
            return;
        }

        Vector2 offset =
            Random.insideUnitCircle * dropRadius;

        Vector3 dropPosition =
            transform.position +
            new Vector3(offset.x, offset.y, 0f);

        GameObject stoneDrop =
            Instantiate(
                stoneDropPrefab,
                dropPosition,
                Quaternion.identity
            );

        ResourcePickup pickup =
            stoneDrop.GetComponent<ResourcePickup>();

        if (pickup != null)
        {
            pickup.SetAmount(stoneDropAmount);
        }

        Debug.Log("Rock dropped stone.");
    }
}