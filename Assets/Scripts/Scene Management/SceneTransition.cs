using System.Collections;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string destinationSpawnID;

    [Header("Trigger Settings")]
    [SerializeField] private bool ignorePlayerAlreadyInside = true;

    private bool initialized;
    private bool ignoreCurrentOccupant;
    private bool transitionStarted;

    private Collider2D triggerCollider;

    private IEnumerator Start()
    {
        triggerCollider =
            GetComponent<Collider2D>();

        // Wait for the Player to be placed at the spawn point.
        yield return new WaitForFixedUpdate();

        if (ignorePlayerAlreadyInside)
        {
            ignoreCurrentOccupant =
                PlayerOrReturningZombieIsInside();
        }

        initialized = true;
    }

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (!initialized)
            return;

        if (transitionStarted)
            return;

        CuredZombieReturnHome zombie =
            other.GetComponentInParent<
                CuredZombieReturnHome
            >();

        bool isPlayer =
            other.transform.root.CompareTag("Player");

        bool isReturningZombie =
            zombie != null &&
            zombie.IsReturningHome;

        if (!isPlayer && !isReturningZombie)
            return;

        // Ignore only the object already standing inside
        // the trigger after scene loading.
        if (ignoreCurrentOccupant)
        {
            ignoreCurrentOccupant = false;

            Debug.Log(
                gameObject.name +
                " ignored the Player already inside it."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(destinationSpawnID))
        {
            Debug.LogError(
                gameObject.name +
                " has no Destination Spawn ID."
            );

            return;
        }

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "SceneTransitionManager.Instance is missing."
            );

            return;
        }

        transitionStarted = true;

        if (isReturningZombie)
        {
            zombie.prepareForHomeBase();
        }

        Debug.Log(
            gameObject.name +
            " is sending destination ID: [" +
            destinationSpawnID +
            "]"
        );

        SceneTransitionManager.Instance.LoadScene(
            sceneToLoad,
            destinationSpawnID
        );
    }

    private bool PlayerOrReturningZombieIsInside()
    {
        if (triggerCollider == null)
            return false;

        Collider2D[] overlappingObjects =
            Physics2D.OverlapBoxAll(
                triggerCollider.bounds.center,
                triggerCollider.bounds.size,
                0f
            );

        foreach (
            Collider2D collider
            in overlappingObjects
        )
        {
            if (collider.transform.root.CompareTag("Player"))
                return true;

            CuredZombieReturnHome zombie =
                collider.GetComponentInParent<
                    CuredZombieReturnHome
                >();

            if (zombie != null &&
                zombie.IsReturningHome)
            {
                return true;
            }
        }

        return false;
    }
}