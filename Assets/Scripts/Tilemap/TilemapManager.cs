using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    private static TilemapManager _instance;
    public static TilemapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TilemapManager>();
            }

            return _instance;
        }
    }

    public Grid grid { get; private set; }
    public Tilemap groundTilemap { get; private set; }
    public Tilemap buildingsTilemap { get; private set; }
    public Tilemap waterTilemap { get; private set; }
    public Tilemap selectionTilemap { get; private set; }

    [SerializeField] private int columns;
    [SerializeField] private int rows;
    [SerializeField] private int additionalWaterTileAmount;
    private List<CellData> cells = new List<CellData>();

    public bool activateClustering;
    public bool activateIsolatedCellsRemoval;
    private void Awake()
    {
        // define tilemaps
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        groundTilemap = grid.transform.Find("GroundTilemap").GetComponent<Tilemap>();
        buildingsTilemap = grid.transform.Find("BuildingTilemap").GetComponent<Tilemap>();
        waterTilemap = grid.transform.Find("WaterTilemap").GetComponent<Tilemap>();
        selectionTilemap = grid.transform.Find("SelectionTilemap").GetComponent<Tilemap>();

        // event to refresh tiles
        BuildingFactory.Instance.updateBuildingTilemapEvent += DispatchBuildingTilemap;
    }

    private void Start()
    {
        GenerateBaseTilemap(columns, rows);

        if (activateIsolatedCellsRemoval)
        {
        RemoveIsolatedCells();
        }

        GeneratorsManager.Instance.ExecuteGenerators();
        DispatchGroundAndBuildingTilemaps();
    }

    public CellData GetCellData(Vector2Int coordinates)
    {
        int index = coordinates.y * (columns + (additionalWaterTileAmount * 2)) + coordinates.x;
        if (!Utils.CellCoordinatesAreValid(coordinates) || index >= cells.Count) return null;
        return cells[index];
    }

    public int GetTilemapColumns() { return columns; }
    public int GetTilemapRows() { return rows; }

    public void GenerateBaseTilemap(int columns, int rows)
    {
        groundTilemap.ClearAllTiles();
        for (int y = 0; y < rows + additionalWaterTileAmount * 2; y++)
        { 
            for (int x = 0; x < columns + additionalWaterTileAmount * 2; x++)
            {

                Vector2Int coordinates = new Vector2Int(x, y);
                CellData cell = new CellData(coordinates);

                if (x <= additionalWaterTileAmount || y <= additionalWaterTileAmount || x > columns + additionalWaterTileAmount || y > rows + additionalWaterTileAmount)
                {
                    cell.environment = Environment.water;
                    cells.Add(cell);
                    continue;
                }

                if (activateClustering)
                {
                    SetTileToCellDependingOnNeighbor(cell);
                    //SetCellDependingOnDistance(cell);
                }
                else
                {
                    SetCellAtRandom(cell);
                }
                cells.Add(cell);
            }
        }
    }

    public void SetCellAtRandom(CellData data)
    {
        int rd = Random.Range(0, 3);
        if (rd == 0)
        {
            SetCellToPlain(data);
        }
        else if (rd == 1)
        {
            SetCellToForest(data);
        }
        else
        {
            SetCellToMountain(data);
        }
    }
    public void SetCellToPlain(CellData data) {
        data.environment = Environment.plain;
    }
    public void SetCellToForest(CellData data) {
        data.environment = Environment.forest;
    }
    public void SetCellToMountain(CellData data) {
        data.environment = Environment.mountain;
    }

    private void SetCellDependingOnDistance(CellData data)
    {
        List<Vector2Int> neighborCoordinates = Utils.GetNeighborOffsetVectors(data.coordinates);
        foreach (Vector2Int neighbor in neighborCoordinates)
        {
            CellData celldata = GetCellData(data.coordinates + neighbor);
            if (celldata != null)
            {
                switch(Utils.GetTileDistance(new Vector2Int(columns/2, rows/2), celldata.coordinates) % 3) {
                    case 0:
                        SetCellToPlain(celldata);
                        break;
                    case 1:
                    SetCellToForest(celldata);
                        break;
                    case 2:
                        SetCellToMountain(celldata);
                        break;
                }
            }
        }
    }

    public void SetTileToCellDependingOnNeighbor(CellData data)
    {
        float plainNeighbors = 0;
        float forestNeighbors = 0;
        float mountainNeighbors = 0;

        List<Vector2Int> neighborCoordinates = Utils.GetNeighborOffsetVectors(data.coordinates);
        foreach (Vector2Int neighbor in neighborCoordinates)
        {   
            CellData celldata = GetCellData(data.coordinates + neighbor);
            if (celldata != null)
            {
                if (celldata.environment == Environment.plain)
                {
                    plainNeighbors += 1;
                }
                else if (celldata.environment == Environment.forest)
                {
                    forestNeighbors += 1;
                }
                else if (celldata.environment == Environment.mountain)
                {
                    mountainNeighbors += 1;
                }
            }
        }
        float total = plainNeighbors + forestNeighbors + mountainNeighbors;
        float plainProba = (plainNeighbors + 1) / (total + 3);
        float forestProba = (forestNeighbors + 1) / (total + 3);
        float mountainProba = (mountainNeighbors + 1) / (total + 3);
        
        double random = Random.value;
        if (random < plainProba) {
            SetCellToPlain(data);
        } else if (random >= plainProba && random < forestProba +plainProba)
        {
            SetCellToForest(data);
        } else
        {
            SetCellToMountain(data);
        }
    }

    public void RemoveIsolatedCells()
    {
        // iterate on each cells
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // for each cell, create a new dictionnary that states how many neighbors of each environement
                Dictionary<Environment, int> neighborAmount = new Dictionary<Environment, int>();
                foreach (Environment environment in Environment.GetValues(typeof(Environment)))
                {
                    // initiate the dictionnary with every environment
                    neighborAmount.Add(environment, 0);
                }


                Vector2Int currentCellCoordinates = new Vector2Int(x, y);
                CellData currentCell = GetCellData(currentCellCoordinates);
                if (currentCell == null || currentCell.environment == null) continue;
                Environment currentCellEnvironment = currentCell.environment.Value;

                List<Vector2Int> neighborCoordinates = Utils.GetNeighborOffsetVectors(currentCellCoordinates);
                foreach (Vector2Int coordinates in neighborCoordinates)
                {
                    CellData neighborCell = GetCellData(currentCellCoordinates + coordinates);
                    if (neighborCell != null && neighborCell.environment != null)
                    {
                        Environment neighborCellEnvironment = neighborCell.environment.Value;
                        neighborAmount[neighborCellEnvironment] += 1;
                    }
                }
                if (neighborAmount[currentCellEnvironment] == 0) // cell is isolated we need to replace it by the environement that is the most present around the cell
                {
                    // get the key of the max value i.e. get the environment that is the most present around the cell
                    KeyValuePair<Environment, int> max = new KeyValuePair<Environment, int>();
                    foreach (KeyValuePair<Environment, int> entry in neighborAmount)
                    {
                        if (entry.Value > max.Value)
                        {
                            max = entry;
                        }
                    }

                    // replace the initial environment by the most present one
                    if (max.Key == Environment.plain)
                    {
                        SetCellToPlain(currentCell);
                    }
                    else if (max.Key == Environment.forest)
                    {
                        SetCellToForest(currentCell);
                    }
                    else if (max.Key == Environment.mountain)
                    {
                        SetCellToMountain(currentCell);
                    }
                }
            }
        }
    }

    public void DispatchGroundAndBuildingTilemaps()
    {
        foreach (CellData cell in cells)
        {
            DispatchTile(cell);
        }
    }

    public void DispatchSelectionTilemap()
    {
        selectionTilemap.ClearAllTiles();
        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();
        if (selectedCell != null)
        {
            selectionTilemap.SetTile(selectedCell.GetVector3Coordinates(), GameAssets.i.selectionTile); 
        }
    }

    private void DispatchBuildingTilemap()
    {
        buildingsTilemap.ClearAllTiles();
        foreach (CellData cell in cells)
        {
            if (cell.building)
            {
                Tile buildingTile = BuildingFactory.Instance.GetBuildingTiles().At(cell.building.type);
                buildingsTilemap.SetTile(cell.GetVector3Coordinates(), buildingTile);
            }
        }
    }

    public void DispatchTile(CellData cellData)
    {
        Environment? cellEnvironment = cellData.environment;
        Tile environmentTile;
        switch (cellEnvironment)
        {
            case null:
                environmentTile = GameAssets.i.fractureTile;
                groundTilemap.SetTile(cellData.GetVector3Coordinates(), environmentTile);
                return;
            case Environment.water:
                environmentTile = BuildingFactory.Instance.GetEnvironmentTiles().At(Environment.water);
                waterTilemap.SetTile(cellData.GetVector3Coordinates(), environmentTile);
                return;
            default:
                environmentTile = BuildingFactory.Instance.GetEnvironmentTiles().At(cellEnvironment.Value);
                groundTilemap.SetTile(cellData.GetVector3Coordinates(), environmentTile);

                Tile buildingTile = cellData.building ? BuildingFactory.Instance.GetBuildingTiles().At(cellData.building.type) : null;
                if (buildingTile == null) return;
                buildingsTilemap.SetTile(cellData.GetVector3Coordinates(), buildingTile);
                return;
        }
        // TODO - display construction asset while the building is not finished.
    }
}