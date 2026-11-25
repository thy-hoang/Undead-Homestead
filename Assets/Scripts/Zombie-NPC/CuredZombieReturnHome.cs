using UnityEngine;
using UnityEngine.SceneManagement;

public class CuredZombieReturnHome : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arrivalDistance = 0.1f;

    [Header("Scene Settings")]
    [SerializeField] private string homeExitTag =
        "HomeBaseExit";

    [SerializeField] private string baseSpawnID =
        "BaseEntrance";

    [SerializeField] private string homeBasePointTag =
        "HomeBasePoint";

    private Rigidbody2D rb;

    private Transform exitTarget;
    private Transform homeBaseTarget;

    private bool returningHome;
    private bool walkingToHomeBase;
    private bool waitingForHomeScene;

    public bool IsReturningHome
    {
        get
        {
            return returningHome;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void beginReturnHome()
    {
        returningHome = true;

        DontDestroyOnLoad(gameObject);

        ZombieFollow follow =
            GetComponent<ZombieFollow>();

        if (follow != null)
        {
            follow.enabled = false;
        }

        ZombieAttack attack =
            GetComponent<ZombieAttack>();

        if (attack != null)
        {
            attack.enabled = false;
        }

        GameObject homeExit =
            GameObject.FindGameObjectWithTag(
                homeExitTag
            );

        if (homeExit == null)
        {
            Debug.LogError(
                "No object tagged " +
                homeExitTag +
                " was found."
            );

            returningHome = false;
            return;
        }

        exitTarget = homeExit.transform;

        Debug.Log(
            gameObject.name +
            " is returning to the home base."
        );
    }

    private void FixedUpdate()
    {
        if (returningHome &&
            exitTarget != null)
        {
            MoveToward(exitTarget);
            return;
        }

        if (walkingToHomeBase &&
            homeBaseTarget != null)
        {
            MoveToward(homeBaseTarget);

            float distance =
                Vector2.Distance(
                    transform.position,
                    homeBaseTarget.position
                );

            if (distance <= arrivalDistance)
            {
                walkingToHomeBase = false;

                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }

                Debug.Log(
                    gameObject.name +
                    " reached the HomeBasePoint."
                );
            }
        }
    }

    private void MoveToward(Transform target)
    {
        Vector2 currentPosition =
            transform.position;

        Vector2 targetPosition =
            target.position;

        Vector2 newPosition =
            Vector2.MoveTowards(
                currentPosition,
                targetPosition,
                moveSpeed *
                Time.fixedDeltaTime
            );

        if (rb != null)
        {
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position = newPosition;
        }
    }

    public void prepareForHomeBase()
    {
        if (waitingForHomeScene)
            return;

        waitingForHomeScene = true;

        SceneManager.sceneLoaded +=
            placeAtHomeBase;
    }

    private void placeAtHomeBase(
        Scene scene,
        LoadSceneMode mode
    )
    {
        SceneManager.sceneLoaded -=
            placeAtHomeBase;

        waitingForHomeScene = false;

        SceneSpawnPoint[] spawnPoints =
            FindObjectsByType<SceneSpawnPoint>(
                FindObjectsSortMode.None
            );

        SceneSpawnPoint matchingSpawn =
            null;

        foreach (
            SceneSpawnPoint spawnPoint
            in spawnPoints
        )
        {
            if (spawnPoint.SpawnID == baseSpawnID)
            {
                matchingSpawn = spawnPoint;
                break;
            }
        }

        if (matchingSpawn == null)
        {
            Debug.LogError(
                "No SceneSpawnPoint with ID '" +
                baseSpawnID +
                "' was found in " +
                scene.name
            );

            return;
        }

        GameObject homePoint =
            GameObject.FindGameObjectWithTag(
                homeBasePointTag
            );

        if (homePoint == null)
        {
            Debug.LogError(
                "No object tagged " +
                homeBasePointTag +
                " was found in " +
                scene.name
            );

            return;
        }

        SceneManager.MoveGameObjectToScene(
            gameObject,
            scene
        );

        Vector3 spawnPosition =
            matchingSpawn.transform.position;

        transform.position = spawnPosition;

        if (rb != null)
        {
            rb.position = spawnPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        homeBaseTarget =
            homePoint.transform;

        returningHome = false;
        walkingToHomeBase = true;

        Debug.Log(
            gameObject.name +
            " spawned at " +
            baseSpawnID +
            " and is walking to HomeBasePoint."
        );
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -=
            placeAtHomeBase;
    }
}