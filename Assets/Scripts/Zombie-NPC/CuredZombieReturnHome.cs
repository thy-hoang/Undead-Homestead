using UnityEngine;
using UnityEngine.SceneManagement;

public class CuredZombieReturnHome : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arrivalDistance = 0.1f;

    [Header("Scene Tags")]
    [SerializeField] private string homeExitTag = "HomeBaseExit";
    [SerializeField] private string baseExitSpawnTag = "BaseExitSpawn";
    [SerializeField] private string homeBasePointTag = "HomeBasePoint";

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

        ZombieFollow follow = GetComponent<ZombieFollow>();
        if (follow != null)
            follow.enabled = false;

        ZombieAttack attack = GetComponent<ZombieAttack>();
        if (attack != null)
            attack.enabled = false;

        GameObject exit =
            GameObject.FindGameObjectWithTag(homeExitTag);

        if (exit == null)
        {
            Debug.LogError(
                "No object tagged " +
                homeExitTag +
                " was found."
            );

            return;
        }

        exitTarget = exit.transform;

        Debug.Log(
            gameObject.name +
            " is walking toward the home-base entrance."
        );
    }

    private void FixedUpdate()
    {
        if (returningHome && exitTarget != null)
        {
            MoveToward(exitTarget);
            return;
        }

        if (walkingToHomeBase && homeBaseTarget != null)
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
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = target.position;

        Vector2 newPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
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

        SceneManager.sceneLoaded += placeAtHomeBase;
    }

    private void placeAtHomeBase(
        Scene scene,
        LoadSceneMode mode
    )
    {
        SceneManager.sceneLoaded -= placeAtHomeBase;

        waitingForHomeScene = false;

        GameObject spawnPoint =
            GameObject.FindGameObjectWithTag(
                baseExitSpawnTag
            );

        GameObject homePoint =
            GameObject.FindGameObjectWithTag(
                homeBasePointTag
            );

        if (spawnPoint == null)
        {
            Debug.LogError(
                "No object tagged " +
                baseExitSpawnTag +
                " was found in BaseScene."
            );

            return;
        }

        if (homePoint == null)
        {
            Debug.LogError(
                "No object tagged " +
                homeBasePointTag +
                " was found in BaseScene."
            );

            return;
        }

        SceneManager.MoveGameObjectToScene(
            gameObject,
            scene
        );

        Vector3 spawnPosition =
            spawnPoint.transform.position;

        transform.position = spawnPosition;

        if (rb != null)
        {
            rb.position = spawnPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        homeBaseTarget = homePoint.transform;

        returningHome = false;
        walkingToHomeBase = true;

        Debug.Log(
            gameObject.name +
            " spawned at BaseExitSpawn and is walking to " +
            homeBaseTarget.position
        );
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= placeAtHomeBase;
    }
}