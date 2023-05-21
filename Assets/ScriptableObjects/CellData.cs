using UnityEngine;
using UnityEngine.Tilemaps;
public class CellData : Object
{
    public Vector3Int coordinates { get; private set; }
    public environments environment { get; set; }
    public Building? building { get; set; }
    public BuildingTypes? buildingType { get; set; }
    public TileBase groundTile { get; set; } = null;
    public TileBase buildingTile { get; set; } = null;
    public TileBase waterTile { get; set; } = null;
    public bool isSelected { get; set; } = false;

    public CellData(Vector3Int coordinates)
    {
        this.coordinates = coordinates;
    }

    public Vector3Int GetCoordinates() {  return coordinates; }
}
