using UnityEngine;

[CreateAssetMenu(menuName = "Objectives/Objective")]
public class Objective : ScriptableObject
{
    public string objectiveID;
    public string objectiveName;
    [TextArea] public string description;

    public Objective parent;

    [Header("Requirements")]
    public string statistic;
    public int targetAmount = 1;
}
