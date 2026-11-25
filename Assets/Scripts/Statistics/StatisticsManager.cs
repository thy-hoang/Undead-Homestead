using UnityEngine;
using System.Collections.Generic;
using System;

public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }

    private Dictionary<string, int> statistics = new();

    public event Action<string, int> OnStatisticChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Increment(string statistic, int amount = 1)
    {
        if (!statistics.ContainsKey(statistic))
            statistics[statistic] = 0;

        statistics[statistic] += amount;

        OnStatisticChanged?.Invoke(
            statistic,
            statistics[statistic]
        );
    }

    public int GetStatistic(string statistic)
    {
        return statistics.GetValueOrDefault(statistic, 0);
    }
}