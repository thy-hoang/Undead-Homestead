using System.Collections.Generic;
using UnityEngine;

public class BaseGridManager : MonoBehaviour
{
    [SerializeField] private int gridWidth = 18;
    [SerializeField] private int gridHeight = 12;

    // The bottom-left cell of the usable area.
    // Adjust these values if your painted floor begins somewhere else.
    [SerializeField] private Vector3Int bottomLeftCell = new Vector3Int(-9, -6, 0);

    private Grid grid;
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public bool IsInsideBase(Vector3Int cell)
    {
        int x = cell.x - bottomLeftCell.x;
        int y = cell.y - bottomLeftCell.y;

        return x >= 0 &&
               x < gridWidth &&
               y >= 0 &&
               y < gridHeight;
    }

    public bool IsCellAvailable(Vector3Int cell)
    {
        return IsInsideBase(cell) && !occupiedCells.Contains(cell);
    }

    public bool TryOccupyCell(Vector3Int cell)
    {
        if (!IsCellAvailable(cell))
            return false;

        occupiedCells.Add(cell);
        return true;
    }

    public void ReleaseCell(Vector3Int cell)
    {
        occupiedCells.Remove(cell);
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return grid.GetCellCenterWorld(cell);
    }
}