using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    public static readonly int DEFAULT_NEIGHBOR_TRAVEL_WEIGHT = 1;
    public static List<CellData> accessibleCells { get; private set; } = new List<CellData>();

    public static void UpdatePathfinderForCell(CellData cell)
    {
        if (accessibleCells.Contains(cell) && !CellIsWalkable(cell))
        {
            accessibleCells.Remove(cell);
            return;
        }

        if (!accessibleCells.Contains(cell) && CellIsWalkable(cell))
        {
            accessibleCells.Add(cell);
            return;
        }
    }

    public static bool CellIsWalkable(CellData cell)
    {
        return cell.environment != Environment.water && cell.building == null;
    }

    public static bool CellIsProperDestination(CellData cell)
    {
        return cell.environment != Environment.water;
    }
}
