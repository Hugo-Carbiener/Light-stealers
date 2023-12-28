using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CellData
{
    public Vector2Int coordinates { get; private set; }
    public Environment environment { get; set; } = Environment.water;
    public Building? building { get; set; }
    public bool isSelected { get; set; } = false;

    public Dictionary<Vector2Int, int> neighborTravelWeight { get; private set; }

    public CellData(Vector2Int coordinates)
    {
        this.coordinates = coordinates;
        neighborTravelWeight = Utils.GetNeighborOffsetVectors(coordinates).ToDictionary(offset => offset, offset => Pathfinder.DEFAULT_NEIGHBOR_TRAVEL_WEIGHT);
    }

    public Vector3Int GetVector3Coordinates() { return new Vector3Int(coordinates.x, coordinates.y, 0); }

    public int GetNeighborTravelWeight(Vector2Int neighborCellCoordinates)
    {
        if (!Utils.CellsAreNeighbors(coordinates, neighborCellCoordinates))
        {
            Debug.LogError(string.Format($"Could not get the neighbor travel weight because target cell ({neighborCellCoordinates.x}, {neighborCellCoordinates.y}) if not neighbor to current cell ({coordinates.x}, {coordinates.y})"));
            return -1;
        }

        return neighborTravelWeight[neighborCellCoordinates - coordinates];
    }
}
