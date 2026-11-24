using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FarmCropRecipe
{
    public string cropName;

    [Header("Inventory Items")]
    public ResourceItem seedItem;
    public ResourceItem cropItem;

    [Header("Temporary Crop Visuals")]
    public Sprite plantedSprite;
    public Sprite grownSprite;

    [Header("Harvest Yield")]
    public int cropYield = 1;
    public int seedYield = 1;
}

public class FarmPlot : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer cropRenderer;

    [Header("Growth")]
    [SerializeField] private float growthDurationSeconds = 180f;

    [Header("Crop Selection Order")]
    [SerializeField]
    private List<FarmCropRecipe> cropRecipes =
        new List<FarmCropRecipe>();

    private FarmCropRecipe plantedCrop;
    private float readyTime;

    private bool isPlanted;
    private bool isGrown;

    public bool IsPlanted => isPlanted;
    public bool IsGrown => isGrown;

    private void Update()
    {
        if (!isPlanted || isGrown)
            return;

        if (Time.time >= readyTime)
        {
            isGrown = true;

            if (cropRenderer != null &&
                plantedCrop != null)
            {
                cropRenderer.sprite =
                    plantedCrop.grownSprite;
            }
        }
    }

    public bool TryPlantFirstAvailable(
        ResourceItemManager resourceManager)
    {
        if (isPlanted ||
            resourceManager == null)
        {
            return false;
        }

        foreach (FarmCropRecipe recipe in cropRecipes)
        {
            if (recipe == null ||
                recipe.seedItem == null ||
                recipe.cropItem == null)
            {
                continue;
            }

            if (recipe.seedItem.quantity <= 0)
                continue;

            plantedCrop = recipe;
            isPlanted = true;
            isGrown = false;

            readyTime =
                Time.time + growthDurationSeconds;

            recipe.seedItem.quantity--;

            if (cropRenderer != null)
            {
                cropRenderer.sprite =
                    recipe.plantedSprite;
            }

            resourceManager.doUpdateDisplay = true;

            Debug.Log(
                $"{recipe.cropName} planted. " +
                $"Growth time: {growthDurationSeconds} seconds."
            );

            return true;
        }

        Debug.Log("No available seeds to plant.");
        return false;
    }

    public bool TryHarvest(
        ResourceItemManager resourceManager)
    {
        if (!isGrown ||
            plantedCrop == null ||
            resourceManager == null)
        {
            return false;
        }

        if (plantedCrop.cropItem != null)
        {
            plantedCrop.cropItem.quantity +=
                plantedCrop.cropYield;
        }

        if (plantedCrop.seedItem != null)
        {
            plantedCrop.seedItem.quantity +=
                plantedCrop.seedYield;
        }

        resourceManager.doUpdateDisplay = true;

        Debug.Log(
            $"Harvested {plantedCrop.cropName}. " +
            $"Added crop and replacement seeds."
        );

        ClearPlot();

        return true;
    }

    private void ClearPlot()
    {
        plantedCrop = null;
        isPlanted = false;
        isGrown = false;
        readyTime = 0f;

        if (cropRenderer != null)
            cropRenderer.sprite = null;
    }
}