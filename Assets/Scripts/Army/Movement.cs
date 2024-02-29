using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Movement : MonoBehaviour
{
    [SerializeField] private Vector2Int destination;
    [SerializeField] private Troop troop;
    [SerializeField]
    private DayNightCyclePhases drawAtStartOfPhase;
    List<CellData> path;

    public void DrawPath(DayNightCyclePhases phaseToDraw)
    {
        if (phaseToDraw != drawAtStartOfPhase) return;
        GeneratePath();
        if (path == null) return;
        CellData prev = null;
        LineRenderer ln;
        if (!TryGetComponent(out ln))
        {
            ln = gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
        }
        Vector3[] positions = path.Select(celldata => TilemapManager.Instance.groundTilemap.layoutGrid.CellToWorld(celldata.GetVector3Coordinates())).ToArray();
        ln.positionCount = positions.Length;
        ln.SetPositions(positions);
        ln.startColor = Color.red;
        ln.endColor = Color.red;
        ln.startWidth = 0.1f;
        ln.endWidth = 0.1f;
        ln.sortingOrder = 10;
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
