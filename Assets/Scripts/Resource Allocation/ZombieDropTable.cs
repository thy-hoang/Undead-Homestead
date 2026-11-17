using UnityEngine;

[System.Serializable]
public class ZombieDrop
{
    public GameObject dropPrefab;

    [Header("Day Settings")]
    [Min(1)]
    public int minimumDay = 1;

    [Min(0)]
    public float startingWeight = 1f;

    [Min(0)]
    public float weightIncreasePerDay = 0f;

    public float GetWeight(int currentDay)
    {
        if (currentDay < minimumDay)
            return 0f;

        int daysSinceUnlock =
            currentDay - minimumDay;

        return startingWeight +
               weightIncreasePerDay * daysSinceUnlock;
    }
}

public class ZombieDropTable : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private ZombieDrop[] possibleDrops;

    [SerializeField] private DayNightCycle dayNightCycle;

    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.75f;

    [SerializeField] private int minimumDrops = 0;
    [SerializeField] private int maximumDrops = 1;

    [SerializeField] private float spawnRadius = 0.3f;

    public void DropLoot(Vector3 deathPosition)
    {
        if (possibleDrops == null ||
            possibleDrops.Length == 0)
        {
            return;
        }

        if (Random.value > dropChance)
            return;

        int currentDay = 1;

        if (dayNightCycle != null)
            currentDay = dayNightCycle.CurrentDay;

        int numberOfDrops = Random.Range(
            minimumDrops,
            maximumDrops + 1
        );

        for (int i = 0; i < numberOfDrops; i++)
        {
            GameObject selectedDrop =
                ChooseRandomDrop(currentDay);

            if (selectedDrop == null)
                continue;

            Vector2 randomOffset =
                Random.insideUnitCircle * spawnRadius;

            Vector3 dropPosition =
                deathPosition +
                new Vector3(
                    randomOffset.x,
                    randomOffset.y,
                    0f
                );

            Instantiate(
                selectedDrop,
                dropPosition,
                Quaternion.identity
            );
        }
    }

    private GameObject ChooseRandomDrop(int currentDay)
    {
        float totalWeight = 0f;

        foreach (ZombieDrop drop in possibleDrops)
        {
            if (drop.dropPrefab == null)
                continue;

            totalWeight += drop.GetWeight(currentDay);
        }

        if (totalWeight <= 0f)
            return null;

        float randomValue =
            Random.Range(0f, totalWeight);

        foreach (ZombieDrop drop in possibleDrops)
        {
            if (drop.dropPrefab == null)
                continue;

            randomValue -= drop.GetWeight(currentDay);

            if (randomValue <= 0f)
                return drop.dropPrefab;
        }

        return null;
    }
}