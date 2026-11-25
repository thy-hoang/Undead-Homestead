
using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour
{
    public float initialSpeed = 2f;
    public float deceleration = 3f;

    private float currentSpeed;
    private TextMeshPro text;

    void Awake()
    {
        currentSpeed = initialSpeed;
        text = GetComponent<TextMeshPro>();
    }

    public void SetDamage(int damage)
    {
        text.text = damage.ToString();
    }

    void Update()
    {
        if (currentSpeed > 0)
        {
            transform.position += Vector3.up * currentSpeed * Time.deltaTime;

            currentSpeed -= deceleration * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
