using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * Rule working on the amount of a specific building.
 */
public class BuildingAmountRule : Rule
{
    protected override List<Comparator> authorizedComparators => new List<Comparator> { Comparator.EQUAL, Comparator.NOT_EQUAL, Comparator.GREATER_THAN, Comparator.LESSER_THAN };

    [SerializeField] private int amount;

    public override bool IsValid(CellData cell, Building building)
    {
        base.ComparatorIsAuthorized();

        int buildingAmount = BuildingFactory.Instance.buildingsConstructed
                                            .Select(building => building.GetBuildingType())
                                            .Count(buildingType => buildingType == building.GetBuildingType());
        switch(comparator)
        {
            case Comparator.EQUAL:
                return amount == buildingAmount;
            case Comparator.NOT_EQUAL:
                return amount != buildingAmount;
            case Comparator.GREATER_THAN:
                return amount > buildingAmount;
            case Comparator.LESSER_THAN:
                return amount < buildingAmount;
            default: 
                return false;
        }
    }
}

