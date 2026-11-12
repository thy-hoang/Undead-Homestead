using UnityEngine;

public class ZombieCureable : MonoBehaviour
{
    public bool IsCured { get; private set; }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color curedColor = Color.green;

    public bool Cure()
    {
        if (IsCured)
            return false;

        IsCured = true;

        ZombieFollow follow =
            GetComponent<ZombieFollow>();

        if (follow != null)
            follow.enabled = false;

        ZombieAttack attack =
            GetComponent<ZombieAttack>();

        if (attack != null)
            attack.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.color = curedColor;

        Debug.Log("Zombie has been cured!");

        return true;
    }
}