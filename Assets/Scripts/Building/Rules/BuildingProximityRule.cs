using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * Verifies the distance of the closest building from the targetted cell.
 */
public class BuildingProximityRule : Rule
{
    [SerializeField] private int nearestBuildingDistance;

    protected override List<Comparator> authorizedComparators => new List<Comparator> { Comparator.EQUAL, Comparator.NOT_EQUAL, Comparator.GREATER_THAN, Comparator.LESSER_THAN };

   public override bool IsValid(CellData cell, Building buildingType)
    {
        ComparatorIsAuthorized();
        int minDistance = BuildingFactory.Instance.buildingsConstructed.Select(building => building.GetPosition())
                                                                        .Where(coordinates => coordinates != cell.coordinates)
                                                                        .Select(buildingCoordinates => Utils.GetTileDistance(cell.coordinates, buildingCoordinates))
                                                                        .Min();
        switch (comparator)
        {
            case Comparator.EQUAL:
                return minDistance == nearestBuildingDistance;
            case Comparator.NOT_EQUAL:
                return minDistance != nearestBuildingDistance;
            case Comparator.GREATER_THAN:
                return minDistance > nearestBuildingDistance;
            case Comparator.LESSER_THAN:
                return minDistance < nearestBuildingDistance;
            default:
                return false;
        }
    }
}
