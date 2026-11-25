using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string destinationSpawnID;

    private bool transitionStarted;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(
            "Transition trigger entered by: " +
            other.name
        );

        if (transitionStarted)
            return;

        CuredZombieReturnHome zombie =
            other.GetComponentInParent<CuredZombieReturnHome>();

        if (zombie != null)
        {
            Debug.Log(
                "Cured zombie found. Returning home: " +
                zombie.IsReturningHome
            );

            if (!zombie.IsReturningHome)
            {
                Debug.LogWarning(
                    "Zombie has CuredZombieReturnHome, " +
                    "but beginReturnHome() was not called."
                );

                return;
            }

            zombie.prepareForHomeBase();

            transitionStarted = true;

            SceneTransitionManager.Instance.LoadScene(
                sceneToLoad,
                destinationSpawnID
            );

            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the transition.");

            transitionStarted = true;

            SceneTransitionManager.Instance.LoadScene(
                sceneToLoad,
                destinationSpawnID
            );
        }
    }
}