using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingAmountRule : Rule
{
    protected override List<Comparator> authorizedComparators => new List<Comparator> { Comparator.EQUAL, Comparator.NOT_EQUAL, Comparator.GREATER_THAN, Comparator.LESSER_THAN };

    [SerializeField] private int amount;

    protected override bool IsValid(CellData cell, Building building)
    {
        base.ComparatorIsAuthorized();

        int buildingAmount = BuildingFactory.Instance.buildingsConstructed
                                            .Select(building => building.type)
                                            .Count(buildingType => buildingType == building.type);
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

