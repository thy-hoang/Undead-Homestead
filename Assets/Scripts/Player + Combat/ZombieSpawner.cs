using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] private float dayZombiesSpawnRate = 2f;
    [SerializeField] private float nightZombiesSpawnRate = 10f;

    [SerializeField] private float distanceOutsideCamera = 1f;

    [Header("References")]
    [SerializeField] private Camera spawnCamera;

    private float spawnTimer;
    public static int ActiveZombieCount { get; private set; }
    private readonly List<GameObject> spawnedZombies = new List<GameObject>();

    private void Start()
    {
        if (spawnCamera == null)
        {
            spawnCamera = Camera.main;
        }
    }

    private void Update()
    {
        PruneDeadZombies();

        if (zombieTypes == null ||
            zombieTypes.Length == 0 ||
            spawnCamera == null)
        {
            return;
        }

        if (zombieTypes == null ||
            zombieTypes.Length == 0 ||
            spawnCamera == null)
        {
            return;
        }

        float currentSpawnRate;

        if (dayNightCycle != null && dayNightCycle.IsNight)
        {
            currentSpawnRate = nightZombiesSpawnRate;
        }
        else
        {
            currentSpawnRate = dayZombiesSpawnRate;
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
        if (spawnedZombies.Count >= maxZombies)
            return;

        GameObject selectedZombie =
            GetRandomZombiePrefab();

        if (selectedZombie == null)
            return;

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(
                "ZombieSpawner could not find an object tagged Player."
            );
            return;
        }

        Vector3 cameraPosition =
            spawnCamera.transform.position;

        float halfHeight =
            spawnCamera.orthographicSize;

        float halfWidth =
            halfHeight * spawnCamera.aspect;

        float leftEdge =
            cameraPosition.x - halfWidth;

        float rightEdge =
            cameraPosition.x + halfWidth;

        float bottomEdge =
            cameraPosition.y - halfHeight;

        float topEdge =
            cameraPosition.y + halfHeight;

        float spawnX;
        float spawnY;

        int randomSide =
            Random.Range(0, 4);

        switch (randomSide)
        {
            case 0:
                spawnX = leftEdge - distanceOutsideCamera;
                spawnY = Random.Range(
                    bottomEdge,
                    topEdge
                );
                break;

            case 1:
                spawnX = rightEdge + distanceOutsideCamera;
                spawnY = Random.Range(
                    bottomEdge,
                    topEdge
                );
                break;

            case 2:
                spawnX = Random.Range(
                    leftEdge,
                    rightEdge
                );
                spawnY = bottomEdge - distanceOutsideCamera;
                break;

            default:
                spawnX = Random.Range(
                    leftEdge,
                    rightEdge
                );
                spawnY = topEdge + distanceOutsideCamera;
                break;
        }

        Vector3 spawnPosition = new Vector3(
            spawnX,
            spawnY,
            selectedZombie.transform.position.z
        );

        GameObject spawnedZombie =
            Instantiate(
                selectedZombie,
                spawnPosition,
                Quaternion.identity
            );

        ZombieFollow zombieFollow =
            spawnedZombie.GetComponent<ZombieFollow>();

        if (zombieFollow == null)
        {
            Debug.LogError(
                spawnedZombie.name +
                " does not have a ZombieFollow component."
            );

            Destroy(spawnedZombie);
            return;
        }

        zombieFollow.SetTarget(player.transform);

        spawnedZombies.Add(spawnedZombie);

        ActiveZombieCount =
            spawnedZombies.Count;

        Debug.Log(
            spawnedZombie.name +
            " spawned and is following " +
            player.name
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

    //function: Awake
    //purpose: this function resets the static counter when the scene starts
    private void Awake()
    {
        ActiveZombieCount = 0;
    }

    //function: OnDestroy
    //purpose: this function resets the static counter when the scene unloads
    private void OnDestroy()
    {
        ActiveZombieCount = 0;
    }

    //function: PruneDeadZombies
    //purpose: this function removes destroyed or deactivated zombies and updates the static counter
    private void PruneDeadZombies()
    {
        for (int i = spawnedZombies.Count - 1; i >= 0; i--)
        {
            GameObject zombie = spawnedZombies[i];

            if (zombie == null || !zombie.activeInHierarchy)
            {
                spawnedZombies.RemoveAt(i);
            }
        }

        ActiveZombieCount = spawnedZombies.Count;
    }
}