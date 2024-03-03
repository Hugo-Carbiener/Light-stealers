using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

/**
 * Generator in charge of creating fractures from which the ennemies will appear
 */
[CreateAssetMenu]
public class FractureGenerator : Generator
{
    [SerializeField] private List<Vector2Int> fracturedCells;

    public override void Initialize()
    {
        initialized = true;
    }

    protected override void GenerateElement()
    {
        ApplyFractures(fracturedCells.Select(position => TilemapManager.Instance.GetCellData(position))
            .Where(element => element != null)
            .ToList());
    }

    private void ApplyFractures(List<CellData> fracturedCells)
    {
        foreach(CellData fracturedCell in fracturedCells)
        {
            CellData cell = TilemapManager.Instance.GetCellData(fracturedCell.coordinates);
            cell.environment = null;
        }
    }
}
