using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class BuildingUIManager : UIManager, ActiveUIInterface
{
    private static readonly string BUTTON_CONTAINER_ELEMENT_KEY = "ButtonContainer";
    private static readonly string BUTTON_ELEMENT_KEY = "Button";
    private static readonly string BUTTON_ICON_KEY = "Icon";
    private static readonly string BUTTON_BUILDING_NAME_KEY = "BuildingName";
    private static readonly string BUTTON_BUILDING_COST_CONTAINER_KEY = "CostContainer";
    private static readonly string BUTTON_COST_FOOD_TEXT_KEY = "FoodAmount";
    private static readonly string BUTTON_COST_WOOD_TEXT_KEY = "WoodAmount";
    private static readonly string BUTTON_COST_STONE_TEXT_KEY = "StoneAmount";

    [Header("Button template")]
    [SerializeField] private VisualTreeAsset button;
    [Header("Icons")]
    [SerializeField] SerializableDictionary<BuildingType, Sprite> iconDictionnary;
    [Header("Deconstruction menu")]
    [SerializeField] private string deconstructionButtonLabel;
    [SerializeField] private Sprite deconstructionIcon;

    private static BuildingUIManager _instance;
    public static BuildingUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BuildingUIManager>();
            }

            return _instance;
        }
    }

    void Start()
    {
        Assert.AreEqual(iconDictionnary.Count(), Enum.GetNames(typeof(BuildingType)).Length);
        root = document.rootVisualElement;
        SetVisibility(DisplayStyle.None);
    }

    // CONSTRUCTION

    /**
     * Add all the necessary buttons to the UI Component.
     */
    public void UpdateBuildingConstructionUIComponent(List<Building> buildingsToDisplay, Vector2Int cellPosition)
    {
         buildingsToDisplay.ForEach(building => AddBuildingButton(building, cellPosition));
    }

    /**
     * Generate a custom button and add it to the UI component.
     */
    private void AddBuildingButton(Building buildingType, Vector2Int cellPosition)
    {
        TemplateContainer buttonToAdd = button.Instantiate();
        InitBuildingButton(buttonToAdd, buildingType);
        VisualElement buttonContainer = root.Q<VisualElement>(BUTTON_CONTAINER_ELEMENT_KEY);
        if (buttonContainer == null)
        {
            Debug.LogError("Could not find Visual element button container in Building construction panel");
            return;
        }

        Button buttonElement = buttonToAdd.Q<Button>(BUTTON_ELEMENT_KEY);
        if (buttonElement == null)
        {
            Debug.LogError("Could not find Button element in Building construction panel button");
            return;
        }
        buttonElement.clickable.clicked += delegate { BuildingFactory.Instance.Build(buildingType.type, cellPosition); };  
        buttonElement.clickable.clicked += delegate { CloseUIComponent(); };
        buttonContainer.Add(buttonToAdd);
    }

    /**
     * Opens the building construction panel and initialise it.
     */
    public void OpenUIComponent()
    {
        ResetUIComponent();
        CellData cell = TileSelectionManager.Instance.GetSelectedCellData();
        if (cell == null)
        {
            Debug.LogError("Attempting to open bulding construction UI menu when no cell is selected.");
            return;
        }

        if (cell.building == null)
        {
            List<Building> buildingsToDisplay = TileSelectionManager.Instance.GetValidBuildings(cell);
            if (buildingsToDisplay.Count == 0)
            {
                CloseUIComponent();
                return;
            }
            UpdateBuildingConstructionUIComponent(buildingsToDisplay, cell.coordinates);
        } else
        {
            UpdateBuildingDeconstructionUIComponent(cell.coordinates);
        }
        SetPosition(TilemapManager.Instance.selectionTilemap.CellToWorld(cell.GetVector3Coordinates()));
        SetVisibility(DisplayStyle.Flex);
    }

    private void InitBuildingButton(TemplateContainer button, Building building)
    {
        VisualElement icon = button.Q<VisualElement>(BUTTON_ICON_KEY);
        Label buildingName = button.Q<Label>(BUTTON_BUILDING_NAME_KEY);
        Label foodAmountLabel = button.Q<Label>(BUTTON_COST_FOOD_TEXT_KEY);
        Label woodAmountLabel = button.Q<Label>(BUTTON_COST_WOOD_TEXT_KEY);
        Label stoneAmountLabel = button.Q<Label>(BUTTON_COST_STONE_TEXT_KEY);

        icon.style.backgroundImage = new StyleBackground(iconDictionnary[building.type]);
        buildingName.text = building.type.ToString();
        foodAmountLabel.text = building.GetCost(ResourceTypes.Food).ToString();
        woodAmountLabel.text = building.GetCost(ResourceTypes.Wood).ToString();
        stoneAmountLabel.text = building.GetCost(ResourceTypes.Stone).ToString();
        
        VisualElement costContainer = button.Q<VisualElement>(BUTTON_BUILDING_COST_CONTAINER_KEY);
        costContainer.style.display = DisplayStyle.Flex;
    }

    // DECONSTRUCTION

    /**
     * Add all the necessary buttons to the UI Component.
     */
    public void UpdateBuildingDeconstructionUIComponent(Vector2Int cellPosition)
    {
        AddDeconstructionButton(cellPosition);
    }

    /**
     * Generate a deconstruction button and add it to the UI component.
     */
    private void AddDeconstructionButton(Vector2Int cellPosition)
    {
        TemplateContainer buttonToAdd = button.Instantiate();
        InitDeconstructionButton(buttonToAdd);
        VisualElement buttonContainer = root.Q<VisualElement>(BUTTON_CONTAINER_ELEMENT_KEY);
        if (buttonContainer == null)
        {
            Debug.LogError("Could not find Visual element button container in Building deconstruction panel");
            return;
        }

        Button buttonElement = buttonToAdd.Q<Button>(BUTTON_ELEMENT_KEY);
        if (buttonElement == null)
        {
            Debug.LogError("Could not find Button element in Building deconstruction panel button");
            return;
        }
        buttonElement.clickable.clicked += delegate { BuildingFactory.Instance.DeconstructBuilding(cellPosition); };
        buttonElement.clickable.clicked += delegate { CloseUIComponent(); };
        buttonContainer.Add(buttonToAdd);
    }

    private void InitDeconstructionButton(TemplateContainer button)
    {
        VisualElement icon = button.Q<VisualElement>(BUTTON_ICON_KEY);
        Label buildingName = button.Q<Label>(BUTTON_BUILDING_NAME_KEY);

        icon.style.backgroundImage = new StyleBackground(deconstructionIcon);
        buildingName.text = deconstructionButtonLabel;

        VisualElement costContainer = button.Q<VisualElement>(BUTTON_BUILDING_COST_CONTAINER_KEY);
        costContainer.style.display = DisplayStyle.None;
    }

    // MISC

    /**
    * Hides the building construction panel.
    */
    public void CloseUIComponent()
    {
        SetVisibility(DisplayStyle.None);
        ResetUIComponent();
    }

    public void ResetUIComponent()
    {
        VisualElement buttonContainer = root.Q<VisualElement>(BUTTON_CONTAINER_ELEMENT_KEY);
        buttonContainer.Clear();
    }

    public void UpdateWorldPosition()
    {
        if (root.style.display == DisplayStyle.None) return;

        Vector3Int cellPosition = TileSelectionManager.Instance.GetSelectedCellData().GetVector3Coordinates();
        Vector3 worldPosition = TilemapManager.Instance.selectionTilemap.CellToWorld(cellPosition);
        SetPosition(worldPosition);
    }
}
