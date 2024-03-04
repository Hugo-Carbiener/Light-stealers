using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * Generator in charge of creating fractures from which the ennemies will appear
 */
[CreateAssetMenu]
public class FractureGenerator : Generator
{
    [SerializeField] private List<Vector2Int> fracturedCells;
    [SerializeField] private GameObject fractureLight;

    public override void Initialize()
    {

        initialized = fracturedCells.Count == 0 || fractureLight != null;
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
            GameObject instantiatedLight = GameObject.Instantiate(fractureLight);
            instantiatedLight.transform.position = TilemapManager.Instance.groundTilemap.layoutGrid.CellToWorld(cell.GetVector3Coordinates());
        }
    }
}
