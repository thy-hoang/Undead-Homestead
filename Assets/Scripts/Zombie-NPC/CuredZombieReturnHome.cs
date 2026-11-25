using UnityEngine;
using UnityEngine.SceneManagement;

public class CuredZombieReturnHome : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Scene Tags")]
    [SerializeField] private string homeExitTag =
        "HomeBaseExit";

    [SerializeField] private string homeBasePointTag =
        "HomeBasePoint";

    private Rigidbody2D rb;
    private Transform exitTarget;

    private bool returningHome;
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

        GameObject exit =
            GameObject.FindGameObjectWithTag(homeExitTag);

        if (exit != null)
        {
            exitTarget = exit.transform;

            Debug.Log(
                gameObject.name +
                " is returning to the home-base entrance."
            );
        }
        else
        {
            Debug.LogError(
                "No object tagged " +
                homeExitTag +
                " was found."
            );
        }
    }

    private void FixedUpdate()
    {
        if (!returningHome || exitTarget == null)
            return;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = exitTarget.position;

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

        GameObject homePoint =
            GameObject.FindGameObjectWithTag(
                homeBasePointTag
            );

        if (homePoint == null)
        {
            Debug.LogError(
                "No object tagged " +
                homeBasePointTag +
                " was found in the home-base scene."
            );

            return;
        }

        transform.position =
            homePoint.transform.position;

        if (rb != null)
        {
            rb.position =
                homePoint.transform.position;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        returningHome = false;

        Debug.Log(
            gameObject.name +
            " arrived at the home base."
        );
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= placeAtHomeBase;
    }
}