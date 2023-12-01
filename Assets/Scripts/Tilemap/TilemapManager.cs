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

    private List<Vector2Int> evenNeighborCoordinates = new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), };
    private List<Vector2Int> oddNeighborCoordinates = new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, -1), };

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

        // add offset to building tilemap
        Vector3 offset = new Vector3(0, buildingsTilemap.layoutGrid.cellSize.x / 4, 0);
        buildingsTilemap.transform.position += offset;
    }

    private void Start()
    {
        GenerateWaterTilemap(columns, rows);
        GenerateGroundTilemap(columns, rows);

        if (activateIsolatedCellsRemoval)
        {
        RemoveIsolatedCells();
        }

        GenerateTownCenter(new Vector2Int(columns / 2, rows / 2));
        DispatchGroundAndBuildingTilemaps();
    }

    public CellData GetCellData(Vector2Int coordinates)
    {
        int index = coordinates.y * columns + coordinates.x;
        if (!CoordinatesAreInBounds(coordinates) || index >= cells.Count) return null;
        return cells[index];
    }

    public int GetTilemapColumns() { return columns; }
    public int GetTilemapRows() { return rows; }

    public void GenerateGroundTilemap(int columns, int rows)
    {
        groundTilemap.ClearAllTiles();
        for (int y = 0; y < rows; y++)
        { 
            for (int x = 0; x < columns; x++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                CellData cell = new CellData(coordinates);

                if (activateClustering)
                {
                    SetTileToCellDependingOnNeighbor(cell);
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

    public void SetTileToCellDependingOnNeighbor(CellData data)
    {
        float plainNeighbors = 0;
        float forestNeighbors = 0;
        float mountainNeighbors = 0;

        // get neighbor coordinates depending on if the tile is even or odd
        List<Vector2Int> neighborCoordinates;

        if (data.coordinates.y % 2 == 0)
        {
            neighborCoordinates = evenNeighborCoordinates;
        }
        else
        {
            neighborCoordinates = oddNeighborCoordinates;
        }
        
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
        List<Vector2Int> neighborCoordinates;

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
                if (currentCell == null) continue;
                Environment currentCellEnvironment = currentCell.environment;
                
                // get neighbor coordinates depending on if the tile is even or odd
                if (y % 2 == 0)
                {
                    neighborCoordinates = evenNeighborCoordinates;
                }
                else
                {
                    neighborCoordinates = oddNeighborCoordinates;
                }

                foreach (Vector2Int coordinates in neighborCoordinates)
                {
                    CellData neighborCell = GetCellData(currentCellCoordinates + coordinates);
                    if (neighborCell != null)
                    {
                        Environment neighborCellEnvironment = neighborCell.environment;
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

    public void GenerateWaterTilemap(int columns, int rows)
    {
        waterTilemap.ClearAllTiles();
        for (int y = -additionalWaterTileAmount; y < rows + additionalWaterTileAmount; y++)
        {
            for (int x = -additionalWaterTileAmount; x < columns + additionalWaterTileAmount; x++)
            {
                waterTilemap.SetTile(new Vector3Int(x, y, 0), GameAssets.i.waterTile);
            }
        }
    }

    public void GenerateTownCenter(Vector2Int coordinates)
    {
        BuildingFactory.Instance.Build(BuildingType.Fountain, coordinates);
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
        Tile environmentTile = BuildingFactory.Instance.GetEnvironmentTiles().At(cellData.environment);
        groundTilemap.SetTile(cellData.GetVector3Coordinates(), environmentTile);

        if (cellData.building != null)
        {
            Tile buildingTile = BuildingFactory.Instance.GetBuildingTiles().At(cellData.building.type);
            buildingsTilemap.SetTile(cellData.GetVector3Coordinates(), buildingTile);
        }
    }

    private bool CoordinatesAreInBounds(Vector2Int coordinates)
    {
        return coordinates.x >= 0 && coordinates.x < columns && coordinates.y >= 0 && coordinates.y < rows;
    }
}