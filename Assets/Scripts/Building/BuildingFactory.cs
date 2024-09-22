using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class BuildingFactory : MonoBehaviour
{
    private static BuildingFactory instance;

    public static BuildingFactory Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<BuildingFactory>();
            }
            return instance;
        }
    }

    [Header("Maps")]
    [SerializeField] private SerializableDictionary<BuildingTypes, GameObject> buildingPrefabs;
    [SerializeField] private SerializableDictionary<BuildingTypes, Tile> buildingTiles;
    [SerializeField] private SerializableDictionary<Environment, Tile> environmentTiles;

    public event Action updateBuildingTilemapEvent;
    
    public List<Building> buildingsConstructed { get; private set; }

    private BuildingTypes previouslyBuiltType;

    private void Awake()
    {
        buildingsConstructed = new List<Building>();
    }


    /**
     * Method called by the radial menu to build a building of given type
     */
    public void Build(BuildingTypes buildingType, Vector2Int coordinates)
    {
        CellData targetCell = TilemapManager.Instance.GetCellData(coordinates);
        if (targetCell == null || targetCell.building) return;
        // instantiate the building prefab and store building information in cell data
        GameObject instantiatedBuilding = Instantiate(
            buildingPrefabs[buildingType], 
            TilemapManager.Instance.grid.GetCellCenterWorld((Vector3Int) targetCell.coordinates) + TilemapManager.Instance.buildingsTilemap.tileAnchor, 
            Quaternion.identity
            );        
        previouslyBuiltType = buildingType;

        Building building;
        if (instantiatedBuilding.TryGetComponent(out building))
        {
            building.SetPosition(targetCell.coordinates);
            building.enabled = true;
        }

        targetCell.building = building;
        buildingsConstructed.Add(building);
    }

    public void DeconstructSelected()
    {
        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();
        DeconstructBuilding(selectedCell.coordinates);
    }

    /**
     * Deconstructs a specific building at specified coordinates
     */
    public void DeconstructBuilding(Vector2Int targetCoordinates)
    {
        foreach (Building building in buildingsConstructed)
        {
            if (building.GetPosition() != targetCoordinates) continue;

            if (!building.CanBeDeconstructed())
            {
                Debug.LogError("Error - Could not deconstruct building " + building.GetBuildingType().ToString() + " because it is protected.");
                continue;
            }

            CellData targetCell = TilemapManager.Instance.GetCellData(targetCoordinates);
            if (targetCell == null)
            {
                Debug.LogError("Error - Could not deconstruct building " + building.GetBuildingType().ToString() + " because its cell data is null.");
                return;
            }

            if (targetCell.building == null)
            {
                Debug.LogError("Error - Could not deconstruct building " + building.GetBuildingType().ToString() + " because its building is null.");
                return;
            }

            targetCell.building = null;
            buildingsConstructed.Remove(building);
            Destroy(building.gameObject);
            TilemapManager.Instance.DispatchTile(targetCell);

            return;
        }
    }
    public SerializableDictionary<BuildingTypes, GameObject> GetBuildingPrefabs() { return buildingPrefabs; }
    public SerializableDictionary<BuildingTypes, Tile> GetBuildingTiles() { return buildingTiles; }
    public SerializableDictionary<Environment, Tile> GetEnvironmentTiles() { return environmentTiles; }
}
