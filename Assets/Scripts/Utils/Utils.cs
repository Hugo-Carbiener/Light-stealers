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

    public static int GetTileDistance(Vector2Int from, Vector2Int to)
    {
        // does not work
        //return ((int) Mathf.Abs(from.x - to.x) + (int) Mathf.Abs(from.y - to.y));

        // works above and under
        int dx = from.x - to.x;     // signed deltas
        int dy = from.y - to.y;
        int x = Mathf.Abs(dx);  // absolute deltas
        int y = Mathf.Abs(dy);
        /*// special case if we start on an odd row or if we move into negative x direction
        if ((dx < 0) ^ ((from.y & 1) == 1))
            x = Mathf.Max(0, x - (y + 1) / 2);
        else
            x = Mathf.Max(0, x - (y) / 2);
        return x + y;*/

        // works
        //int penalty = ((from.y % 2 == 0 && to.y % 2 == 1 && from.x < to.x) || (to.y % 2 == 0 && from.y % 1 == 1 && to.x < from.x)) ? 1 : 0;
        //return Mathf.Max(y, x + (int) Mathf.Floor(y / 2) + penalty);

        int penalty = ((from.y % 2 == 0 && to.y % 2 == 1 && from.x < to.x) || (to.y % 2 == 0 && from.y % 2 == 1 && to.x < from.x)) ? 1 : 0;
        return Mathf.Max(y,  x + (int)Mathf.Floor(y / 2) + penalty);
    }
}
