using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Terraformer
{
    private static readonly List<Environment> unterraformableEnvironments = new List<Environment>(){Environment.city, Environment.water};

    public static void Terraform(Vector2Int coordinates)
    {
        CellData targetCell = TilemapManager.Instance.GetCellData(coordinates);
        if (targetCell == null 
            || targetCell.environment == null 
            || targetCell.building
            || unterraformableEnvironments.Contains(targetCell.environment.Value)) return;

        targetCell.environment = Environment.city;
        TilemapManager.Instance.DispatchTile(targetCell);
    }

    public static void TerraformAround(Vector2Int coordinates)
    {
        List<Vector2Int> neighborCoordinates = Utils.GetNeighborOffsetVectors(coordinates);
        foreach(Vector2Int neighbor in neighborCoordinates)
        {
            Terraform(coordinates + neighbor);
        }
    }
}
