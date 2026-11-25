using UnityEngine;
using UnityEngine.InputSystem;

public class FarmPlotInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ResourceItemManager resourceManager;

    [Header("Farm Plot Detection")]
    [SerializeField] private float detectionRadius = 1f;

    private void Awake()
    {
        if (resourceManager == null)
        {
            resourceManager =
                FindAnyObjectByType<ResourceItemManager>();
        }
    }

    private void Update()
    {
        bool interactPressed =
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame;

        if (Gamepad.current != null &&
            Gamepad.current.buttonNorth.wasPressedThisFrame)
        {
            interactPressed = true;
        }

        if (!interactPressed)
            return;

        Debug.Log(
            "FarmPlotInteraction received Interact."
        );

        HandleFarmInteraction();
    }

    private void HandleFarmInteraction()
    {
        FarmPlot farmPlot =
            FindFarmPlotUnderPlayer();

        if (farmPlot == null)
        {
            Debug.Log(
                "Interact pressed, but no FarmPlot was detected."
            );

            return;
        }

        // Harvest first if the crop is mature.
        if (farmPlot.TryHarvest(resourceManager))
            return;

        // Otherwise, plant the first available seed.
        farmPlot.TryPlantFirstAvailable(resourceManager);
    }

    public bool IsStandingOnFarmPlot()
    {
        return FindFarmPlotUnderPlayer() != null;
    }

    private FarmPlot FindFarmPlotUnderPlayer()
    {
        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                transform.position,
                detectionRadius
            );

        foreach (Collider2D collider in colliders)
        {
            FarmPlot farmPlot =
                collider.GetComponentInParent<FarmPlot>();

            if (farmPlot != null)
                return farmPlot;
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius
        );
    }
}