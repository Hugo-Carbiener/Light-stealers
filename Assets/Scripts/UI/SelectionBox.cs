using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private GameObject tileContainer;
    [SerializeField] private GameObject buildingContainer;

    [Header("Components to update")]
    [SerializeField] private Text tileCoordinatesText;
    [SerializeField] private Text tileEnvironmentText;
    [SerializeField] private Text buildingProductionText;
    [SerializeField] private Image tileImage;
    [SerializeField] private Image buildingImage;

    private CellData cellData;
    private bool selectionIsDisplayed;

    private void Start()
    {
        tilemapManager = TilemapManager.Instance;
        if (!tileContainer) tileContainer = transform.GetChild(0).gameObject;
        if (!buildingContainer) buildingContainer = transform.GetChild(1).gameObject;
        tileContainer.SetActive(false);
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
            tileContainer.SetActive(true);
        }
        else
        {
            tileContainer.SetActive(false);
            buildingContainer.SetActive(false);
        }        
    }

    private void updateData()
    {
        cellData = tilemapManager.getSelectedCellData();
        // bool in GetComponent required to get inactive component
        tileEnvironmentText.text = "Environment: " + cellData.environment.ToString();
        tileCoordinatesText.text = "Coordinates: " + cellData.coordinates.x + ", " + cellData.coordinates.y;

        if (cellData.building == null)
        {
            buildingContainer.SetActive(false);
        } else {
            buildingProductionText.text = "Building produces " + ProductionManager.Instance.getBuildingsProductionAmounts().At((BuildingTypes)cellData.building)
                + "units of " + ProductionManager.Instance.getBuildingsProductions().At((BuildingTypes)cellData.building);
            buildingContainer.SetActive(true);
        }

        switch (cellData.environment)
        {
            case environments.plain:
                tileImage.sprite = GameAssets.i.plainTileSprite;
                break;
            case environments.forest:
                tileImage.sprite = GameAssets.i.forestTileSprite;
                break;
            case environments.mountain:
                tileImage.sprite = GameAssets.i.mountainTileSprite;
                break;
        }

        switch(cellData.building)
        {
            case BuildingTypes.Sawmill:
                buildingImage.sprite = GameAssets.i.sawmill;
                break;
            case BuildingTypes.Windmill:
                buildingImage.sprite = GameAssets.i.windmill;
                break;
        }
    }
}
