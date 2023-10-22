using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private GameObject selectionBoxContainer;
    [SerializeField] private GameObject buildingContainer;

    [Header("Components to update")]
    [SerializeField] private Text tileCoordinatesText;
    [SerializeField] private Text tileEnvironmentText;
    [SerializeField] private Text buildingProductionText;
    [SerializeField] private Text buildingStatusText;
    [SerializeField] private Image tileImage;
    [SerializeField] private Image buildingImage;

    private CellData cellData;
    private bool selectionIsDisplayed;

    private void Start()
    {
        tilemapManager = TilemapManager.Instance;
        if (!buildingContainer) buildingContainer = transform.GetChild(1).gameObject;
        selectionBoxContainer.SetActive(false);
        buildingContainer.SetActive(false);

        cellData = tilemapManager.getSelectedCellData();
    }

    private void Update()
    {
        // enable the selection box if a cell is selected, otherwise disables it
        selectionIsDisplayed = tilemapManager.selectionIsDisplayed();

        if (selectionIsDisplayed)
        {
            updateData();
            selectionBoxContainer.SetActive(true);
        }
        else
        {
            selectionBoxContainer.SetActive(false);
        }        
    }

    private void updateData()
    {
        cellData = tilemapManager.getSelectedCellData();
        // bool in GetComponent required to get inactive component
        tileEnvironmentText.text = "Environment: " + cellData.environment.ToString();
        tileCoordinatesText.text = "Coordinates: " + cellData.coordinates.x + ", " + cellData.coordinates.y;

        if (cellData.buildingType == null)
        {
            buildingContainer.SetActive(false);
        } else {
            buildingProductionText.text = "Building produces " + ProductionManager.Instance.getBuildingsProductionAmounts().At((BuildingType)cellData.buildingType)
                + "units of " + ProductionManager.Instance.getBuildingsProductions().At((BuildingType)cellData.buildingType);
            buildingStatusText.text = "Building status : " + cellData.building.activated.ToString();
            buildingContainer.SetActive(true);
        }

        switch (cellData.environment)
        {
            case Environment.plain:
                tileImage.sprite = GameAssets.i.plainTileSprite;
                break;
            case Environment.forest:
                tileImage.sprite = GameAssets.i.forestTileSprite;
                break;
            case Environment.mountain:
                tileImage.sprite = GameAssets.i.mountainTileSprite;
                break;
        }

        switch(cellData.buildingType)
        {
            case BuildingType.Sawmill:
                buildingImage.sprite = GameAssets.i.sawmill;
                break;
            case BuildingType.Farm:
                buildingImage.sprite = GameAssets.i.farm;
                break;
        }
    }
}
