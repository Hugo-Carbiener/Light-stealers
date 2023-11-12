using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UIElements;

public class TileSelectionManager : MonoBehaviour
{
    private Vector2Int frameSelect = new Vector2Int(0, 0);

    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private Tilemap selectionTilemap;
    private CellData selectedCell;

    private static TileSelectionManager _instance;
    public static TileSelectionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TileSelectionManager>();
            }

            return _instance;
        }
    }

    private void Start()
    {
        tilemapManager = TilemapManager.Instance;
        selectionTilemap = tilemapManager.selectionTilemap;
        InputManager.onSelectInput += SelectCell;
    }

    private void SelectCell(Vector2 mousePosition)
    {
        Vector2Int oldFrameSelect = new Vector2Int(0, 0);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2Int tilePos = (Vector2Int) selectionTilemap.WorldToCell(worldPos);
        oldFrameSelect = frameSelect;
        frameSelect = tilePos;

        SetSelectCell(tilePos);
        tilemapManager.DispatchSelectionTilemap();
        UpdateBuildingConstructionUI();
    }

    /**
     * Select a cell if not cell is currently selected, unselect it otherwise. 
     */
    private void SetSelectCell(Vector2Int coordinates)
    {
        if (selectedCell != null && selectedCell.GetCoordinates() == coordinates)
        {
            // unselect cell
            selectedCell = null;
            return;
        } 

        selectedCell = tilemapManager.getCellData(coordinates);
    }

    public CellData GetSelectedCellData() { return selectedCell; }

    /**
     * Fetch the ist of avalibale buildings and display the building construction panel.
     */
    private void UpdateBuildingConstructionUI()
    {
        if (selectedCell == null)
        {
            CloseBuildingConstructionUI();
            return;
        }

        List<BuildingType> buildingsAvailable = GetValidBuildings(selectedCell);
        if (buildingsAvailable.Count == 0)
        {
            CloseBuildingConstructionUI();
            return;
        }

        BuildingConstructionUIManager buildingConstructionUI = BuildingConstructionUIManager.Instance;
        buildingConstructionUI.ResetUIComponent();
        buildingConstructionUI.UpdateUIComponent(buildingsAvailable);
        buildingConstructionUI.setPosition(selectionTilemap.CellToWorld(selectedCell.GetVector3Coordinates()));
        buildingConstructionUI.setVisibility(DisplayStyle.Flex);
    }

    /**
     * Hides the building construction panel.
     */
    private void CloseBuildingConstructionUI()
    {
        BuildingConstructionUIManager buildingConstructionUI = BuildingConstructionUIManager.Instance;
        buildingConstructionUI.setVisibility(DisplayStyle.None);
        buildingConstructionUI.ResetUIComponent();
    }

    /**
     * Generates the list of building types that are valid to be built on a given tile.
     */
    private List<BuildingType> GetValidBuildings(CellData cell)
    {
        List<BuildingType> validBuildings = new List<BuildingType>();
        SerializableDictionary<BuildingType, GameObject> buildingPrefabs = BuildingFactory.Instance.GetBuildingPrefabs();
        Dictionary<BuildingType, GameObject> buildingPrefabsDictionnary = buildingPrefabs.ToDictionnary();

        foreach(GameObject building in buildingPrefabsDictionnary.Values)
        {
            if (!building.TryGetComponent<Building>(out Building buildingComponent)) continue;

            Rule[] buildingRules = building.GetComponentsInChildren<Rule>();
            int invalidBuildingRuleAmount = buildingRules.Where(rule => !rule.IsValid(cell, buildingComponent)).Count();

            if (invalidBuildingRuleAmount > 0) continue;
            validBuildings.Add(buildingComponent.type);
        }

        return validBuildings;
    }
}
