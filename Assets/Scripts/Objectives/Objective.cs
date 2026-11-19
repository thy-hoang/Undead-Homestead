using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Objectives/Objective")]
public class Objective : ScriptableObject
{
    public Objective parentObjective;
    public string objectiveName;
    public string description;

    public string statistic;
    public int targetAmount;
}
