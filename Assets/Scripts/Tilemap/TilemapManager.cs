using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager Instance { get; private set; }

    public Grid grid { get; private set; }
    public Tilemap groundTilemap { get; private set; }
    public Tilemap buildingsTilemap { get; private set; }
    public Tilemap waterTilemap { get; private set; }
    public Tilemap selectionTilemap { get; private set; }

    [SerializeField] private int columns;
    [SerializeField] private int rows;
    private List<CellData> cells = new List<CellData>();

    private bool displaySelection = false;
    // true when the value of selectedCell just changed
    private CellData selectedCell;
    // field may be defined by previously selected cell
    private List<Tile> tiles;

    public bool activateClustering;
    public bool activateIsolatedCellsRemoval;

    private List<Vector3Int> evenNeighborCoordinates = new List<Vector3Int>() { new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), };
    private List<Vector3Int> oddNeighborCoordinates = new List<Vector3Int>() { new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0), };

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
        
        // create tile list
        tiles = new List<Tile>();
        foreach (Tile tile in GameAssets.i.plainTiles)
        {
            tiles.Add(tile);
        }
        foreach (Tile tile in GameAssets.i.forestTiles)
        {
            tiles.Add(tile);
        }
        foreach (Tile tile in GameAssets.i.mountainTiles)
        {
            tiles.Add(tile);
        }

        // create instance of tilemapManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Debug.Log("You had to destroy a TilemapManager You fucked up");
        }

        // add offset to building tilemap
        Vector3 offset = new Vector3(0, buildingsTilemap.layoutGrid.cellSize.x / 4, 0);
        buildingsTilemap.transform.position += offset;
    }

    private void Start()
    {
        generateGroundTilemap(columns, rows);
        if (activateIsolatedCellsRemoval)
        {
        removeIsolatedCells();
        }
        //mergeTiles();
        generateCastle();

        // --------------
        initialPaintTilemap();
    }

    private void Update()
    {
        DispatchSelectionTilemap();
    }

    public int? getCell(Vector3Int coordinates)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].coordinates == coordinates)
            {
                return i;
            }
        }
        return null;
    }

    public CellData getCellData(Vector3Int coordinates)
    {
        foreach (CellData data in cells)
        {
            if (data.coordinates == coordinates)
            {
                return data;
            }
        }
        return null;
    }

    public int getTilemapColumns() { return columns; }
    public int getTilemapRows() { return rows; }

    public bool selectionIsDisplayed() { return displaySelection; }
    public CellData getSelectedCellData() { return selectedCell; }

    // ------------------------------------------------
    // ------------------------------------------------
    public void generateGroundTilemap(int columns, int rows)
    {
        // Start on a blank grid
        groundTilemap.ClearAllTiles();
        buildingsTilemap.ClearAllTiles();
        waterTilemap.ClearAllTiles();
        selectionTilemap.ClearAllTiles();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3Int coordinates = new Vector3Int(x, y, 0);
                CellData cell = new CellData(coordinates);

                if (activateClustering)
                {
                    setTileToCellDependingOnNeighbor(cell);
                }
                else
                {
                    setCellAtRandom(cell);
                }
                cells.Add(cell);
            }
        }
    }
    public void SelectCell(Vector3Int coordinates)
    {
        displaySelection = true;
        int? cellIndex = getCell(coordinates);
        selectedCell = cells[(int)cellIndex];
    }

    public void reSelectCell ()
    {
        displaySelection = !displaySelection;
    }

    public void setCellAtRandom(CellData data)
    {
        int rd = Random.Range(0, 3);
        if (rd == 0)
        {
            setCellToPlain(data);
        }
        else if (rd == 1)
        {
            setCellToForest(data);
        }
        else
        {
            setCellToMountain(data);
        }
    }
    public void setCellToPlain(CellData data) {
        data.environment = environments.plain;
        data.groundTile = GameAssets.i.basePlainTile;
    }
    public void setCellToForest(CellData data) {
        data.environment = environments.forest;
        data.groundTile = GameAssets.i.baseForestTile;
    }
    public void setCellToMountain(CellData data) {
        data.environment = environments.mountain;
        data.groundTile = GameAssets.i.baseMountainTile;
    }

    public void setTileToCellDependingOnNeighbor(CellData data)
    {
        float plainNeighbors = 0;
        float forestNeighbors = 0;
        float mountainNeighbors = 0;

        // get neighbor coordinates depending on if the tile is even or odd
        List<Vector3Int> neighborCoordinates;

        if (data.coordinates.y % 2 == 0)
        {
            neighborCoordinates = evenNeighborCoordinates;
        }
        else
        {
            neighborCoordinates = oddNeighborCoordinates;
        }
        
        foreach (Vector3Int neighbor in neighborCoordinates)
        {   
            int? currentCellIndex = getCell(data.coordinates + neighbor);
            if (currentCellIndex != null)
            {
                if (cells[ (int) currentCellIndex].environment == environments.plain)
                {
                    plainNeighbors += 1;
                }
                else if (cells[ (int) currentCellIndex].environment == environments.forest)
                {
                    forestNeighbors += 1;
                }
                else if (cells[ (int) currentCellIndex].environment == environments.mountain)
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
            setCellToPlain(data);
        } else if (random >= plainProba && random < forestProba +plainProba)
        {
            setCellToForest(data);
        } else
        {
            setCellToMountain(data);
        }
    }

    public void removeIsolatedCells()
    {
        List<Vector3Int> neighborCoordinates;

        // iterate on each cells
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // for each cell, create a new dictionnary that states how many neighbors of each environement
                Dictionary<environments, int> neighborAmount = new Dictionary<environments, int>();
                foreach (environments environment in environments.GetValues(typeof(environments)))
                {
                    // initiate the dictionnary with every environment
                    neighborAmount.Add(environment, 0);
                }


                Vector3Int currentCellCoordinates = new Vector3Int(x, y, 0);
                int? currentCellIndex = getCell(currentCellCoordinates);
                CellData currentCell = cells[(int)currentCellIndex];
                environments currentCellEnvironment = cells[(int)currentCellIndex].environment;

                // get neighbor coordinates depending on if the tile is even or odd
                if (y % 2 == 0)
                {
                    neighborCoordinates = evenNeighborCoordinates;
                }
                else
                {
                    neighborCoordinates = oddNeighborCoordinates;
                }

                foreach (Vector3Int coordinates in neighborCoordinates)
                {
                    int? neighborCellIndex = getCell(currentCellCoordinates + coordinates);
                    if (neighborCellIndex != null)
                    {
                    environments neighborCellEnvironment = cells[(int)neighborCellIndex].environment;
                    neighborAmount[neighborCellEnvironment] += 1;
                    }
                }
                if (neighborAmount[currentCellEnvironment] == 0) // cell is isolated we need to replace it by the environement that is the most present around the cell
                {
                    // get the key of the max value i.e. get the environment that is the most present around the cell
                    KeyValuePair<environments, int> max = new KeyValuePair<environments, int>();
                    foreach (KeyValuePair<environments, int> entry in neighborAmount)
                    {
                        if (entry.Value > max.Value)
                        {
                            max = entry;
                        }
                    }

                    // replace the initial environment by the most present one
                    if (max.Key == environments.plain)
                    {
                        setCellToPlain(currentCell);
                    }
                    else if (max.Key == environments.forest)
                    {
                        setCellToForest(currentCell);
                    }
                    else if (max.Key == environments.mountain)
                    {
                        setCellToMountain(currentCell);
                    }
                }
            }
        }
    }

    public void mergeTiles()
    {
        string tileName;
        List<Vector3Int> neighborCoordinates;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3Int currentPosition = new Vector3Int(x, y, 0);
                int? currentCellIndex = getCell(currentPosition);
                environments currentCellEnvironment = cells[ (int) currentCellIndex].environment;
                tileName = currentCellEnvironment.ToString() + "tile";

                // get neighbor coordinates depending on if the tile is even or odd
                if ( y % 2 == 0)
                {
                    neighborCoordinates = evenNeighborCoordinates;
                } else
                {
                    neighborCoordinates = oddNeighborCoordinates;
                }

                for (int i = 0; i < 6; i++)
                {
                    int? neighborCellIndex = getCell(currentPosition + neighborCoordinates[i]);
                    if (neighborCellIndex != null)
                    {
                        environments neighborCellEnvironment = cells[ (int) neighborCellIndex].environment;
                        if (currentCellEnvironment == neighborCellEnvironment)
                        {
                            // if current tile and neighbor are from the same env, we add the current neighbor number (1 to 6) to the end of the tile name.
                            tileName += i + 1;
                        }
                    }
                }
                
            foreach(Tile tile in tiles)
            {
                //Debug.Log("merging : tile name is " + tile.name);
                if(tile.name == tileName)
                {
                    cells[ (int) currentCellIndex].groundTile = (TileBase) tile;
                }
            }
            }
        }
    }

    public void generateCastle()
    {
        Vector3Int center = new Vector3Int(rows/2, columns/2, 0);
        int? centerCellIndex = getCell(center);
        CellData centerCell = cells[(int)centerCellIndex];
        centerCell.waterTile = GameAssets.i.waterTiles[7];
        List<Vector3Int> neighborCoordinates;
        int i = 0;
        

        if (center.y % 2 == 0)
        {
            neighborCoordinates = evenNeighborCoordinates;
        }
        else
        {
            neighborCoordinates = oddNeighborCoordinates;
        }

        foreach (Vector3Int neighbor in neighborCoordinates)
        {
            int? currentCellIndex = getCell(center + neighbor);
            CellData currentCell = cells[(int)currentCellIndex];
            i += 1;
            currentCell.waterTile = GameAssets.i.waterTiles[i];
        }
    }

    public void initialPaintTilemap()
    // if a water tile is referred to in the CellData it will be painted and the ground/building tile ignored
    {
        foreach (CellData cell in cells)
        {
            if (cell.waterTile == null)
            {
                if (cell.buildingTile != null)
                {
                    buildingsTilemap.SetTile(cell.coordinates, cell.buildingTile);
                }
                if (cell.groundTile != null)
                {
                    groundTilemap.SetTile(cell.coordinates, cell.groundTile);
                }
            }
            else
            {
                waterTilemap.SetTile(cell.coordinates, cell.waterTile);
            }
        }
    }

    public void DispatchSelectionTilemap()
    {
        selectionTilemap.ClearAllTiles();
        if(displaySelection)
        {
            selectionTilemap.SetTile(selectedCell.coordinates, GameAssets.i.selectionTile); 
        }
    }

    private void DispatchBuildingTilemap()
    {
        foreach (CellData cell in cells)
        {
            if (cell.buildingTile)
            {
                buildingsTilemap.SetTile(cell.coordinates, cell.buildingTile);
            }
        }
    }

    public void DispatchTile(Vector3Int coordinates)
    {
        CellData data = getCellData(coordinates);
        groundTilemap.SetTile(coordinates, data.groundTile);
        buildingsTilemap.SetTile(coordinates, data.buildingTile);
    }
}