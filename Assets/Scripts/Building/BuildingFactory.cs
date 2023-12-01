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
    [SerializeField] private SerializableDictionary<BuildingType, GameObject> buildingPrefabs;
    [SerializeField] private SerializableDictionary<BuildingType, Tile> buildingTiles;
    [SerializeField] private SerializableDictionary<Environment, Tile> environmentTiles;

    public event Action updateBuildingTilemapEvent;
    
    public List<Building> buildingsConstructed { get; private set; }

    private BuildingType previouslyBuiltType;

    private void Awake()
    {
        buildingsConstructed = new List<Building>();
    }


    /**
     * Method called by the radial menu to build a building of given type
     */
    public void Build(BuildingType buildingType, Vector2Int coordinates)
    {
        CellData targetCell = TilemapManager.Instance.GetCellData(coordinates);
        if (targetCell == null || targetCell.building) return;

        // if the player does not have the required amount of resources we cannot build
        ResourceManager resourceManager = ResourceManager.Instance;
        Building prefabBuilding = buildingPrefabs.At(buildingType).GetComponent<Building>();
        foreach (ResourceTypes resourceType in Enum.GetValues(typeof(ResourceTypes)))
        {
            int cost = prefabBuilding.GetCost(resourceType);
            int resource = resourceManager.getResource(resourceType);
            if (resource < cost)
            {
                // player does not have the funds to pay for construction
                return;
            }
        }

        // pay the build
        PayBuild(buildingType);

        previouslyBuiltType = buildingType;

        // instantiate the building prefab and store building information in cell data
        GameObject instantiatedBuilding = Instantiate(buildingPrefabs.At(buildingType));        

        Building building;
        if (instantiatedBuilding.TryGetComponent(out building)) {
            building.SetCoordinates(targetCell.coordinates);
            building.OnConstructionFinished.AddListener(NotifyProductionManager);
            building.OnConstructionFinished.AddListener(NotifyTilemapManager);
        }

        targetCell.building = building;
        buildingsConstructed.Add(building);

        // TODO 
        // add building in construction sprite update

        building.StartConstruction();
    }

    private void NotifyProductionManager()
    {
        // we store the information we built a new 
        ProductionManager.Instance.AddBuilding(previouslyBuiltType);
    }

    private void NotifyTilemapManager()
    {
        // set the buildings values in the selected cell data and the coordinates in the building data if there is no building already placed
        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();
        TilemapManager.Instance.DispatchTile(selectedCell);
    }
    
    private void PayBuild(BuildingType buildingType)
    {
        ResourceManager resourceManager = ResourceManager.Instance;
        Building prefabBuilding = buildingPrefabs.At(buildingType).GetComponent<Building>();
        foreach (ResourceTypes resourceType in Enum.GetValues(typeof(ResourceTypes)))
        {
            int cost = prefabBuilding.GetCost(resourceType);
            int resource = resourceManager.getResource(resourceType);
            resourceManager.modifyResources(resourceType, -cost);
        }
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
            if (building.GetCoordinates() == targetCoordinates)
            {
                if (building.CanBeDeconstructed())
                {
                    CellData targetCell = TilemapManager.Instance.GetCellData(targetCoordinates);

                    // update production data
                    Dictionary<BuildingType, int> buildingsAmount = ProductionManager.Instance.getBuildingAmount();
                    buildingsAmount[targetCell.building.type] -= 1;

                    // update cell data
                    targetCell.building = null;

                    // update scene
                    Destroy(building.gameObject);

                    TilemapManager.Instance.DispatchTile(targetCell);

                    return;
                } else
                {
                    Debug.Log("could not be deconstructed");
                }
            }
        }
    }

    public SerializableDictionary<BuildingType, GameObject> GetBuildingPrefabs() { return buildingPrefabs; }
    public SerializableDictionary<BuildingType, Tile> GetBuildingTiles() { return buildingTiles; }
    public SerializableDictionary<Environment, Tile> GetEnvironmentTiles() { return environmentTiles; }
}
