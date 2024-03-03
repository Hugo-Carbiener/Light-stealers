using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    * Rules ensure certain conditions are valid to enable the construction of a building.
    */
public abstract class Rule : MonoBehaviour

{
    [SerializeField] protected Comparator comparator;

    protected abstract List<Comparator> authorizedComparators { get; }

    public abstract bool IsValid(CellData cell, Building buildingType);

    protected bool ComparatorIsAuthorized()
    {
        return Comparator.NONE.Equals(comparator) && authorizedComparators.Count == 0
            || authorizedComparators.Contains(comparator);
    }
}
