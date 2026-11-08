using System.Collections.Generic;
using UnityEngine;

public class BaseGridManager : MonoBehaviour
{
    [SerializeField] private int gridWidth = 18;
    [SerializeField] private int gridHeight = 12;

    [SerializeField]
    private Vector3Int bottomLeftCell = new Vector3Int(-9, -6, 0);

    private Grid grid;

    private HashSet<Vector3Int> occupiedCells =
        new HashSet<Vector3Int>();

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
        return IsInsideBase(cell) &&
               !occupiedCells.Contains(cell);
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

    public bool IsAreaInsideBase(
        Vector3Int bottomLeft,
        Vector2Int size)
    {
        if (size.x <= 0 || size.y <= 0)
            return false;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = bottomLeft +
                    new Vector3Int(x, y, 0);

                if (!IsInsideBase(cell))
                    return false;
            }
        }

        return true;
    }

    public bool IsAreaAvailable(
        Vector3Int bottomLeft,
        Vector2Int size)
    {
        if (!IsAreaInsideBase(bottomLeft, size))
            return false;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = bottomLeft +
                    new Vector3Int(x, y, 0);

                if (occupiedCells.Contains(cell))
                    return false;
            }
        }

        return true;
    }

    public bool TryOccupyArea(
        Vector3Int bottomLeft,
        Vector2Int size)
    {
        if (!IsAreaAvailable(bottomLeft, size))
            return false;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = bottomLeft +
                    new Vector3Int(x, y, 0);

                occupiedCells.Add(cell);
            }
        }

        return true;
    }

    public void ReleaseArea(
        Vector3Int bottomLeft,
        Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = bottomLeft +
                    new Vector3Int(x, y, 0);

                occupiedCells.Remove(cell);
            }
        }
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return grid.GetCellCenterWorld(cell);
    }

    public Vector3 AreaToWorld(
        Vector3Int bottomLeft,
        Vector2Int size)
    {
        Vector3 bottomLeftCenter =
            grid.GetCellCenterWorld(bottomLeft);

        Vector3 topRightCenter =
            grid.GetCellCenterWorld(
                bottomLeft +
                new Vector3Int(size.x - 1, size.y - 1, 0));

        return (bottomLeftCenter + topRightCenter) / 2f;
    }

    public bool TryFindFirstAvailableArea(
    Vector2Int size,
    out Vector3Int bottomLeft)
    {
        for (int x = bottomLeftCell.x;
             x <= bottomLeftCell.x + gridWidth - size.x;
             x++)
        {
            for (int y = bottomLeftCell.y;
                 y <= bottomLeftCell.y + gridHeight - size.y;
                 y++)
            {
                Vector3Int testCell =
                    new Vector3Int(x, y, 0);

                if (IsAreaAvailable(testCell, size))
                {
                    bottomLeft = testCell;
                    return true;
                }
            }
        }

        bottomLeft = Vector3Int.zero;
        return false;
    }
}