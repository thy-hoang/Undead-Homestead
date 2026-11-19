using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private StatisticsManager statisticsManager;
    private List<Objective> allObjectives;

    private Dictionary<Objective, ObjectiveInstance> objectiveInstances = new();
    private Dictionary<Objective, List<Objective>> children = new();

    public event Action<ObjectiveInstance> OnObjectiveUpdated;

    private void Start()
    {
        InitializeObjectives();

        statisticsManager.OnStatisticChanged += OnStatisticChanged;
    }

    private void OnDestroy()
    {
        if (statisticsManager != null)
            statisticsManager.OnStatisticChanged -= OnStatisticChanged;
    }

    private void InitializeObjectives()
    {
        allObjectives = Resources.LoadAll<Objective>("Objectives").ToList();

        Debug.Log(
            "Loaded objectives: " +
            allObjectives.Count
        );

        // Create runtime instances
        foreach (Objective objective in allObjectives)
        {
            if (objective == null)
                continue;

            objectiveInstances[objective] =
                new ObjectiveInstance(objective);

            children[objective] = new List<Objective>();
        }

        // Build parent-child relationships
        foreach (Objective objective in allObjectives)
        {
            if (objective == null || objective.parent == null)
                continue;

            if (children.TryGetValue(objective.parent, out var childList))
                childList.Add(objective);
            else
                Debug.LogWarning(
                    $"Objective Manager: Parent of {objective.name} is missing from the objective list."
                );
        }

        // Activate root objectives
        foreach (Objective objective in allObjectives)
        {
            if (objective == null || objective.parent != null)
                continue;

            ActivateObjective(objective);
        }
    }

    private void OnStatisticChanged(string statistic, int value)
    {
        foreach (ObjectiveInstance instance in objectiveInstances.Values)
        {
            if (instance.state != ObjectiveState.Active)
                continue;

            if (instance.objective.statistic != statistic)
                continue;

            EvaluateObjective(instance);
        }
    }

    private void ActivateObjective(Objective objective)
    {
        ObjectiveInstance instance = objectiveInstances[objective];

        if (instance.state != ObjectiveState.Locked)
            return;

        instance.state = ObjectiveState.Active;

        OnObjectiveUpdated?.Invoke(instance);

        // Check whether its requirements are already satisfied
        EvaluateObjective(instance);
    }

    private void EvaluateObjective(ObjectiveInstance instance)
    {
        Objective objective = instance.objective;

        int currentAmount =
            statisticsManager.GetStatistic(objective.statistic);

        instance.progress = Mathf.Min(
            currentAmount,
            objective.targetAmount
        );

        if (currentAmount >= objective.targetAmount)
        {
            CompleteObjective(instance);
        }
        else
        {
            OnObjectiveUpdated?.Invoke(instance);
        }
    }

    private void CompleteObjective(ObjectiveInstance instance)
    {
        if (instance.state == ObjectiveState.Completed)
            return;

        instance.state = ObjectiveState.Completed;
        instance.progress = instance.objective.targetAmount;

        OnObjectiveUpdated?.Invoke(instance);

        // Unlock children
        if (!children.TryGetValue(instance.objective, out var childList))
            return;

        foreach (Objective child in childList)
        {
            ActivateObjective(child);
        }

        Debug.Log(
            $"ObjectiveManager: Completed objective {instance.objective.name}"
        );
    }

    public ObjectiveInstance GetObjective(Objective objective)
    {
        return objectiveInstances.GetValueOrDefault(objective);
    }

    public List<ObjectiveInstance> GetActiveObjectives()
    {
        return objectiveInstances.Values
            .Where(x => x.state == ObjectiveState.Active)
            .ToList();
    }
}