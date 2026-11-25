using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string destinationSpawnID;

    [Header("Trigger Settings")]
    [SerializeField] private bool ignorePlayerAtSpawn = true;
    [SerializeField] private float spawnIgnoreDistance = 0.3f;

    private bool transitionStarted;

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (transitionStarted)
            return;

        Transform enteringRoot =
            other.transform.root;

        bool isPlayer =
            enteringRoot.CompareTag("Player");

        CuredZombieReturnHome zombie =
            other.GetComponentInParent<
                CuredZombieReturnHome
            >();

        bool isReturningZombie =
            zombie != null &&
            zombie.IsReturningHome;

        if (!isPlayer && !isReturningZombie)
            return;

        // Ignore only the Player being placed directly
        // on this object's spawn point.
        if (isPlayer &&
            ignorePlayerAtSpawn &&
            PlayerIsAtThisSpawnPoint(enteringRoot))
        {
            Debug.Log(
                gameObject.name +
                " ignored activation because the Player " +
                "was placed at this spawn point."
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

    private bool PlayerIsAtThisSpawnPoint(
        Transform playerRoot
    )
    {
        SceneSpawnPoint spawnPoint =
            GetComponent<SceneSpawnPoint>();

        if (spawnPoint == null)
            return false;

        float distance =
            Vector2.Distance(
                playerRoot.position,
                spawnPoint.transform.position
            );

        return distance <= spawnIgnoreDistance;
    }
}