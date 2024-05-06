using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FightUtils : MonoBehaviour
{
    public static List<Unit> GetUnitsOnCell(Vector2Int location)
    {
        List<Unit> belligerents = UnitManager.Instance.activeUnits.Values.SelectMany(element => element).ToList();
        return belligerents.Where(element => element != null)
            .Where(element => element.GetMovementModule().currentCell == location)
            .ToList();
    }

    public static List<FightModule> GetBelligerentsOnCell(Vector2Int location)
    {
        List<FightModule> unitsOnCell = GetUnitsOnCell(location).Select(unit => unit.GetFightModule()).ToList();
        CellData cell = TilemapManager.Instance.GetCellData(location);
        if (cell == null) return unitsOnCell;

        if (cell.building && cell.building.GetFightModule().IsAttackable())
        {
            unitsOnCell.Add(cell.building.GetFightModule());
        }

        return unitsOnCell;
    }
}
