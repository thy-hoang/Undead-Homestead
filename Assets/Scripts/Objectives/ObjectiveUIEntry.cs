
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveUIEntry : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text objectiveName;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text progress;

    [Header("State Sprites")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite completedSprite;

    [Header("Objective References")]
    [SerializeField] private string objectiveID;
    [SerializeField] private ObjectiveManager objectiveManager;

    private void Start()
    {
        objectiveManager = FindFirstObjectByType<ObjectiveManager>();

        if (objectiveManager == null)
        {
            Debug.LogError("ObjectiveManager not found in scene!");
            return;
        }
        
        ObjectiveInstance instance = objectiveManager.GetObjective(objectiveID);

        if (instance == null)
        {
            Debug.LogError($"Objective not found: {objectiveID}");
            return;
        }

        objectiveName.text = instance.objective.objectiveName;
        description.text = instance.objective.description;

        objectiveManager.OnObjectiveUpdated += OnObjectiveUpdated;

        UpdateAppearance(instance);
    }

    private void OnObjectiveUpdated(ObjectiveInstance instance)
    {
        if (instance.objective.objectiveID != objectiveID)
            return;

        UpdateAppearance(instance);
    }

    private void UpdateAppearance(ObjectiveInstance instance)
    {
        switch (instance.state)
        {
            case ObjectiveState.Locked:
                background.sprite = lockedSprite;
                break;

            case ObjectiveState.Active:
                background.sprite = activeSprite;
                break;

            case ObjectiveState.Completed:
                background.sprite = completedSprite;
                break;
        }

        if (progress != null)
        {
            progress.text =
                $"{instance.progress} / {instance.objective.targetAmount}";
        }
    }

    private void OnDestroy()
    {
        if (objectiveManager != null)
            objectiveManager.OnObjectiveUpdated -= OnObjectiveUpdated;
    }
}
