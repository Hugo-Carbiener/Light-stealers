using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static readonly List<Vector2Int> evenNeighborCoordinates = new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), };
    private static readonly List<Vector2Int> oddNeighborCoordinates = new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, -1), };

    public static List<Vector2Int> GetNeighborOffsetVectors(Vector2Int coordinates)
    {
        // get neighbor coordinates depending on if the tile is even or odd
        List<Vector2Int> neighborCoordinates;

        if (coordinates.y % 2 == 0)
        {
            neighborCoordinates = evenNeighborCoordinates;
        }
        else
        {
            neighborCoordinates = oddNeighborCoordinates;
        }

        return neighborCoordinates;
    }
}
