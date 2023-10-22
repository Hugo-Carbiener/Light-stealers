using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelectionManager : MonoBehaviour
{
    private Vector2Int frameSelect = new Vector2Int(0, 0);

    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private Tilemap selectionTilemap;
    private bool cellIsSelected;

    private void Start()
    {
        tilemapManager = TilemapManager.Instance;
        selectionTilemap = tilemapManager.selectionTilemap;
        InputManager.onSelectInput += updateFrameSelect;
        cellIsSelected = false;
    }

    private void updateFrameSelect(Vector2 mousePosition)
    {
        Vector2Int oldFrameSelect = new Vector2Int(0, 0);

        // Cell cannot be unselected or reselected selected if menu is opened on a selected cell
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2Int tilePos = (Vector2Int) selectionTilemap.WorldToCell(worldPos);
        oldFrameSelect = frameSelect;
        frameSelect = tilePos;

        if (frameSelect == oldFrameSelect)
        {
            tilemapManager.reSelectCell();
            cellIsSelected = false;
        }
        else
        {
            // generate newly selected cell coordinates
            tilemapManager.SelectCell(tilePos);
            cellIsSelected = true;
            
        }
    }
}
