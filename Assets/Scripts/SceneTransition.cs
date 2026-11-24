using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneTransitionManager.Instance.LoadScene(
                sceneToLoad
            );

            return;
        }

        CuredZombieReturnHome curedZombie =
            other.GetComponentInParent<CuredZombieReturnHome>();

        if (curedZombie != null &&
            curedZombie.IsReturningHome)
        {
            curedZombie.prepareForHomeBase();

            SceneTransitionManager.Instance.LoadScene(
                sceneToLoad
            );
        }
    }
}