using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string destinationSpawnID;

    private bool transitionStarted;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (transitionStarted)
            return;

        CuredZombieReturnHome zombie =
            other.GetComponentInParent<CuredZombieReturnHome>();

        if (zombie != null &&
            zombie.IsReturningHome)
        {
            zombie.prepareForHomeBase();
        }

        if (other.CompareTag("Player") ||
            zombie != null &&
            zombie.IsReturningHome)
        {
            transitionStarted = true;

            SceneTransitionManager.Instance.LoadScene(
                sceneToLoad,
                destinationSpawnID
            );
        }
    }
}