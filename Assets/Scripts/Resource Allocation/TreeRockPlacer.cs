using UnityEngine;
using UnityEngine.Tilemaps;
public class TreeRockPlacer : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private GameObject shortTreePrefab;
    [SerializeField] private GameObject tallTreePrefab;
    [SerializeField] private GameObject rockPrefab;

    void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap reference is missing!");
        }

        // Randomly place trees and rocks on the tilemap only on grass tiles
        RandomlyPlaceObjects();

    }

    bool IsGrassTile(TileBase tile)
    {
        // check if the tile name ends in "1_10", "1_15", or "1_16"
        if (tile == null) return false;
        string tileName = tile.name;
        return tileName.EndsWith("1_10") || tileName.EndsWith("1_15") || tileName.EndsWith("1_16");
    }

    void RandomlyPlaceObjects()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (IsGrassTile(tile))
                {
                    // Randomly decide to place a tree or rock
                    float randomValue = Random.value;

                    Vector3Int cellPosition = new Vector3Int(x + bounds.x, y + bounds.y, 0);
                    Vector3 spawnPosition = tilemap.GetCellCenterWorld(cellPosition);
                    Quaternion spawnRotation = Quaternion.identity;

                    if (randomValue < 0.005f) // 5% chance for a tall tree
                    {
                        Instantiate(tallTreePrefab, spawnPosition, spawnRotation, transform);
                    }
                    else if (randomValue < 0.015f) // 10% chance for a short tree
                    {
                        Instantiate(shortTreePrefab, spawnPosition, spawnRotation, transform);
                    }
                    else if (randomValue < 0.020f) // 5% chance for a rock
                    {
                        Instantiate(rockPrefab, spawnPosition, spawnRotation, transform);
                    }
                }
            }
        }
    }

}
