using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelectionManager : MonoBehaviour
{
    private Vector3Int frameSelect = new Vector3Int(0, 0, 0);

    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private RadialMenuController radialMenu;
    private bool cellIsSelected;

    private void Start()
    {
        if (!radialMenu) radialMenu = GameObject.Find("Radial Menu Canvas").GetComponent<RadialMenuController>();

        tilemapManager = TilemapManager.Instance;
        selectionTilemap = tilemapManager.selectionTilemap;
        InputManager.onSelectInput += updateFrameSelect;
        InputManager.onRightClick += SwitchMenu;
        cellIsSelected = false;
    }

    private void updateFrameSelect(Vector2 mousePosition)
    {
        Vector3Int oldFrameSelect = new Vector3Int(0, 0, 0);
        if (!radialMenu.MenuIsOpened()) { 
        // Cell cannot be unselected or reselected selected if menu is opened on a selected cell
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int tilePos = selectionTilemap.WorldToCell(worldPos);
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

    public void SwitchMenu()
    {
        // menu can be open and closed if a cell is selected
        bool menuIsOpened = radialMenu.MenuIsOpened();
        if (cellIsSelected)
        {
            if (!menuIsOpened)
            {
                environments menuEnvironment = tilemapManager.getSelectedCellData().environment;
                radialMenu.OpenMenu(frameSelect, menuEnvironment);
            }
            else
            {
                radialMenu.CloseMenu();
            }
        }
    }
}
