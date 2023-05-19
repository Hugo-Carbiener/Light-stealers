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
    [SerializeField] private SerializableDictionary<BuildingTypes, GameObject> buildingsPrefabs;
    public event Action updateBuildingTilemapEvent;
    private TilemapManager tilemapManager;
    
    public List<Building> buildingsConstructed { get; private set; }

    private BuildingTypes previouslyBuiltType;

    private void Awake()
    {
        buildingsConstructed = new List<Building>();
    }

    private void Start()
    {
        tilemapManager = TilemapManager.Instance;
    }
    
    /**
     * Method called by the radial menu to build a building of given type
     */
    public void build(BuildingTypes buildingType)
    {
        CellData selectedCell = tilemapManager.getSelectedCellData();
        
        // if the selected tile does have a building we cannot build another
        if (selectedCell.buildingTile) return;

        // if the player does not have the required amount of resources we cannot build
        ResourceManager resourceManager = ResourceManager.Instance;
        Building prefabBuilding = buildingsPrefabs.At(buildingType).GetComponent<Building>();
        foreach (ResourceTypes resourceType in Enum.GetValues(typeof(ResourceTypes)))
        {
            int cost = prefabBuilding.getCost(resourceType);
            int resource = resourceManager.getResource(resourceType);
            if (resource < cost)
            {
                // player does not have the funds to pay for construction
                return;
            }
        }

        // pay the build
        payBuild(buildingType);

        previouslyBuiltType = buildingType;

        // instantiate the building prefab and store building information in cell data
        GameObject instantiatedBuilding = Instantiate(buildingsPrefabs.At(buildingType));        

        Building building;
        if (instantiatedBuilding.TryGetComponent(out building)) {
            building.setCoordinates(selectedCell.coordinates);
            building.OnConstructionFinished.AddListener(notifyProductionManager);
            building.OnConstructionFinished.AddListener(notifyTilemapManager);
        }

        selectedCell.buildingType = buildingType;
        selectedCell.building = building;
        buildingsConstructed.Add(building);

        // TODO 
        // add building in construction sprite update

        building.startConstruction();
    }

    private void notifyProductionManager()
    {
        // we store the information we built a new 
        ProductionManager.Instance.AddBuilding(previouslyBuiltType);
    }

    private void notifyTilemapManager()
    {
        // set the buildings values in the selected cell data and the coordinates in the building data if there is no building already placed
        CellData selectedCell = tilemapManager.getSelectedCellData();
        foreach (Tile tile in GameAssets.i.buildingTiles)
        {
            if (tile.name.Equals(previouslyBuiltType.ToString()))
            {
                selectedCell.buildingTile = tile;
            }
        }
        tilemapManager.UpdateTile(selectedCell.coordinates);
    }
    
    private void payBuild(BuildingTypes buildingType)
    {
        ResourceManager resourceManager = ResourceManager.Instance;
        Building prefabBuilding = buildingsPrefabs.At(buildingType).GetComponent<Building>();
        foreach (ResourceTypes resourceType in Enum.GetValues(typeof(ResourceTypes)))
        {
            int cost = prefabBuilding.getCost(resourceType);
            int resource = resourceManager.getResource(resourceType);
            resourceManager.modifyResources(resourceType, -cost);
        }
    }

    public void deconstructSelected()
    {
        CellData selectedCell = tilemapManager.getSelectedCellData();
        deconstructBuilding(selectedCell.coordinates);
    }

    /**
     * Deconstructs a specific building at specified coordinates
     */
    public void deconstructBuilding(Vector3Int targetCoordinates)
    {
        foreach (Building building in buildingsConstructed)
        {
            if (building.getCoordinates() == targetCoordinates)
            {
                if (building.canBeDeconstructed())
                {
                    CellData targetCell = tilemapManager.getCellData(targetCoordinates);

                    // update production data
                    Dictionary<BuildingTypes, int> buildingsAmount = ProductionManager.Instance.getBuildingAmount();
                    buildingsAmount[(BuildingTypes)targetCell.buildingType] -= 1;

                    // update cell data
                    targetCell.buildingType = null;
                    targetCell.buildingTile = null;

                    // update scene
                    Destroy(building.gameObject);

                    tilemapManager.UpdateTile(targetCoordinates);

                    return;
                } else
                {
                    Debug.Log("could not be deconstructed");
                }
            }
        }
    }
}
