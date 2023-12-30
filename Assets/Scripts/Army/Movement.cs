using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private int maxTraverRange;
    [SerializeField] private Vector2Int destination;
    [SerializeField] private Troop troop;
    List<CellData> path;

    public void DrawPath(List<CellData> draw)
    {
        GeneratePath();
        if (path == null) return;
        CellData prev = null;
        path.ForEach(cell =>
        {
            if (prev == null) return;
            Debug.DrawLine(TilemapManager.Instance.groundTilemap.CellToWorld(prev.GetVector3Coordinates()), TilemapManager.Instance.groundTilemap.CellToWorld(cell.GetVector3Coordinates()), Color.white, 2.5f);
            prev = cell;
        });
    }

    private void GeneratePath()
    {
        path = Pathfinder.GetPath(troop.position, destination);
        
    }

    private void Start()
    {
        DayNightCycleManager.OnCyclePhaseStart += DrawPath;
    }
}
