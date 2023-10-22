using UnityEngine;
using UnityEngine.Tilemaps;
public class CellData : Object
{
    public Vector2Int coordinates { get; private set; }
    public environments environment { get; set; }
    public Building? building { get; set; }
    public BuildingTypes? buildingType { get; set; }
    public TileBase groundTile { get; set; } = null;
    public TileBase buildingTile { get; set; } = null;
    public TileBase waterTile { get; set; } = null;
    public bool isSelected { get; set; } = false;

    public CellData(Vector2Int coordinates)
    {
        this.coordinates = coordinates;
    }

    public Vector2Int GetCoordinates() {  return coordinates; }
    public Vector3Int GetVector3Coordinates() { return new Vector3Int(coordinates.x, coordinates.y, 0); }
}
