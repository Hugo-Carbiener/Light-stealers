using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Generator in charge of creating the town center
 */
[CreateAssetMenu]
public class TownCenterGenerator : Generator
{
    [SerializeField] private Vector2Int position;
    [SerializeField] private BuildingTypes buildingType;

    public override void Initialize()
    {
        initialized = true;
        if (!Utils.CellCoordinatesAreValid(position))
        {
            Debug.LogError("Town center generator could not be initialized on unvalid coordinates.");
            initialized = false;
            return;
        }

        CellData cell = TilemapManager.Instance.GetCellData(position);
        if (cell == null || cell?.environment == null )
        {
            Debug.LogError("Town center generator could not be initialized on null cell or environment.");
            initialized = false;
            return;
        }
    }

    protected override void GenerateElement()
    {
        CellData targetCell = TilemapManager.Instance.GetCellData(position);
        targetCell.environment = Environment.city;
        BuildingFactory.Instance.Build(buildingType, position);
    }
}
