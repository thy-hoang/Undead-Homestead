using UnityEngine;

public class ZombieCureable : MonoBehaviour
{
    public bool IsCured { get; private set; }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color curedColor = Color.green;

    private GameObject zombieRoot;

    private void Awake()
    {
        zombieRoot = transform.root.gameObject;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public bool CureAndReturn(string sceneName)
    {
        if (IsCured)
            return false;

        IsCured = true;

        if (spriteRenderer != null)
            spriteRenderer.color = curedColor;

        CuredZombieReturnHome returnHome =
            zombieRoot.GetComponent<CuredZombieReturnHome>();

        if (returnHome == null)
        {
            Debug.LogError(
                "CuredZombieReturnHome is missing from " +
                zombieRoot.name
            );

            return false;
        }

        returnHome.beginReturnHome();

        Debug.Log(
            zombieRoot.name +
            " was cured and is returning home."
        );

        return true;
    }
}