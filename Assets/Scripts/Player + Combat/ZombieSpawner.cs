using UnityEngine;

[System.Serializable]
public class ZombieSpawnEntry
{
    public GameObject prefab;

    [Min(0)]
    public float startingWeight = 1f;

    [Min(0)]
    public float weightIncreasePerDay = 0f;

    [Min(1)]
    public int minimumDay = 1;

    public float GetWeight(int currentDay)
    {
        if (currentDay < minimumDay)
            return 0f;

        return startingWeight +
               (currentDay - minimumDay) * weightIncreasePerDay;
    }
}

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Settings")]
    [SerializeField] private ZombieSpawnEntry[] zombieTypes;
    [SerializeField] private int maxZombies = 10;

    [Header("Spawn Rate")]
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private float dayZombiesPerMinute = 2f;
    [SerializeField] private float nightZombiesPerMinute = 10f;

    [SerializeField] private float distanceOutsideCamera = 1f;

    [Header("References")]
    [SerializeField] private Camera spawnCamera;

    private float spawnTimer;

    private void Start()
    {
        if (spawnCamera == null)
        {
            spawnCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (zombieTypes == null ||
            zombieTypes.Length == 0 ||
            spawnCamera == null)
        {
            return;
        }

        float currentSpawnRate;

        if (dayNightCycle != null && dayNightCycle.IsNight)
        {
            currentSpawnRate = nightZombiesPerMinute;
        }
        else
        {
            currentSpawnRate = dayZombiesPerMinute;
        }

        if (currentSpawnRate <= 0f)
            return;

        float spawnInterval = 60f / currentSpawnRate;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer -= spawnInterval;
            SpawnZombie();
        }
    }

    private void SpawnZombie()
    {
        int currentZombieCount =
            FindObjectsByType<ZombieHealth>(
                FindObjectsSortMode.None
            ).Length;

        if (currentZombieCount >= maxZombies)
            return;

        GameObject selectedZombie = GetRandomZombiePrefab();

        if (selectedZombie == null)
            return;

        Vector3 cameraPosition = spawnCamera.transform.position;

        float halfHeight = spawnCamera.orthographicSize;
        float halfWidth = halfHeight * spawnCamera.aspect;

        float leftEdge = cameraPosition.x - halfWidth;
        float rightEdge = cameraPosition.x + halfWidth;
        float bottomEdge = cameraPosition.y - halfHeight;
        float topEdge = cameraPosition.y + halfHeight;

        float spawnX;
        float spawnY;

        int randomSide = Random.Range(0, 4);

        switch (randomSide)
        {
            case 0:
                spawnX = leftEdge - distanceOutsideCamera;
                spawnY = Random.Range(bottomEdge, topEdge);
                break;

            case 1:
                spawnX = rightEdge + distanceOutsideCamera;
                spawnY = Random.Range(bottomEdge, topEdge);
                break;

            case 2:
                spawnX = Random.Range(leftEdge, rightEdge);
                spawnY = bottomEdge - distanceOutsideCamera;
                break;

            default:
                spawnX = Random.Range(leftEdge, rightEdge);
                spawnY = topEdge + distanceOutsideCamera;
                break;
        }

        Vector3 spawnPosition = new Vector3(
            spawnX,
            spawnY,
            selectedZombie.transform.position.z
        );

        Instantiate(
            selectedZombie,
            spawnPosition,
            Quaternion.identity
        );
    }

    private GameObject GetRandomZombiePrefab()
    {
        int currentDay = 1;

        if (dayNightCycle != null)
        {
            currentDay = dayNightCycle.CurrentDay;
        }

        float totalWeight = 0f;

        foreach (ZombieSpawnEntry entry in zombieTypes)
        {
            if (entry.prefab != null)
            {
                totalWeight += entry.GetWeight(currentDay);
            }
        }

        if (totalWeight <= 0f)
            return null;

        float randomValue = Random.Range(0f, totalWeight);

        foreach (ZombieSpawnEntry entry in zombieTypes)
        {
            if (entry.prefab == null)
                continue;

            randomValue -= entry.GetWeight(currentDay);

            if (randomValue <= 0f)
                return entry.prefab;
        }

        return null;
    }
}