using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    * Verifies the environment of the targetted cell.
    */
public class TileEnvironmentRule : Rule
{
    [SerializeField] private Environment environment;

    protected override List<Comparator> authorizedComparators => new List<Comparator> { Comparator.EQUAL, Comparator.NOT_EQUAL };

    protected override bool IsValid(CellData cell, Building buildingType)
    {
        ComparatorIsAuthorized();

        Environment cellEnvironment = cell.environment;
        switch (comparator)
        {
            case Comparator.EQUAL:
                return environment.Equals(cellEnvironment);
            case Comparator.NOT_EQUAL:
                return !environment.Equals(cellEnvironment);
            default:
                return false;
        }
    }
}
